using Bot.Services;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Threading.Tasks;

namespace Bot.Modules
{
    public class MoviesModule : ModuleBase<SocketCommandContext>
    {
        public RadarrService radarrService { get; set; }

        [Command("movie", true)]
        public Task UserAsync() => SendLookupDetails();

        async Task SendLookupDetails()
        {
            await Context.Channel.TriggerTypingAsync();

            var contents = Context.Message.Content.Split(' ').Skip(1).ToArray();
            var searchTerm = string.Join(" ", contents);
            var searchResults = await radarrService.Lookup(searchTerm);

            foreach (var movie in searchResults)
            {
                var embed = new EmbedBuilder();
                embed.WithAuthor($"Searching for {searchTerm}", "https://github.com/Radarr/radarr.github.io/blob/master/img/logo-256.png?raw=true");
                embed.WithCurrentTimestamp();
                embed.WithColor(0xfca103);
                embed.AddField(movie.title, movie.year);

                var message = await ReplyAsync(embed: embed.Build());
                await message.AddReactionAsync(new Emoji("❌"));
                await message.AddReactionAsync(new Emoji("✅"));
            }

        }
    }
}
