using Bot.Data;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using LiteDB.Async;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Bot.Services
{
    public class CommandHandlingService
    {
        private readonly LiteDatabaseAsync database;
        private readonly IServiceProvider provider;
        private readonly CommandService commands;

        public CommandHandlingService(
            IServiceProvider provider,
            CommandService commands,
            DatabaseService databaseService)
        {
            database = databaseService.Database;
            this.provider = provider;
            this.commands = commands;
        }

        public async Task InitializeAsync()
        {
            await commands.AddModulesAsync(Assembly.GetEntryAssembly(), provider);
        }

        public async Task MessageReceived(SocketMessage rawMessage, DiscordSocketClient discord)
        {
            // Ignore system messages and messages from bots
            if (rawMessage is not SocketUserMessage message) return;
            if (message.Source != MessageSource.User) return;

            int argPos = 0;
            //if (!message.HasMentionPrefix(discord.CurrentUser, ref argPos)) return;
            if (!message.HasStringPrefix("!", ref argPos)) return;

            var context = new SocketCommandContext(discord, message);
            var result = await commands.ExecuteAsync(context, argPos, provider);

            if (result.Error.HasValue &&
                result.Error.Value == CommandError.UnknownCommand)
                return;

            if (result.Error.HasValue &&
                result.Error.Value != CommandError.UnknownCommand)
                await context.Channel.SendMessageAsync(result.ToString());

            // Add points to the user for using the bot
            // Do this asynchronously, on another task, to prevent database access (and levelup notifications?) from halting the bot
            _ = UpdateLevelAsync(context);
        }

        private async Task UpdateLevelAsync(SocketCommandContext context)
        {
            var users = database.GetCollection<User>("users");
            var user = await users.FindOneAsync(u => u.Id == context.User.Id) ?? new User { Id = context.User.Id };

            ++user.Points;
            await users.UpsertAsync(user);
        }
    }
}
