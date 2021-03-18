using Bot.Services;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Workers
{
    public class AutomationWorkerOptions
    {
        public string Token { get; set; }
        public ulong AutomationChannelId { get; set; }
    }

    public class AutomationWorker : BackgroundService
    {
        private readonly ILogger<AutomationWorker> logger;
        private readonly AutomationWorkerOptions options;
        private readonly DiscordSocketClient discord;
        private readonly CommandHandlingService commandHandlingService;
        private readonly SonarrService sonarrService;
        private readonly RadarrService radarrService;

        public AutomationWorker(
            ILogger<AutomationWorker> logger,
            IOptions<AutomationWorkerOptions> options,
            DiscordSocketClient discord,
            CommandHandlingService commandHandlingService,
            SonarrService sonarrService,
            RadarrService radarrService)
        {
            this.logger = logger;
            this.discord = discord;
            this.commandHandlingService = commandHandlingService;
            this.options = options.Value;
            this.sonarrService = sonarrService;
            this.radarrService = radarrService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(1000, cancellationToken);

                if (discord.ConnectionState != ConnectionState.Connected)
                {
                    continue;
                }

                // Get one message to process
                var channel = discord.GetChannel(options.AutomationChannelId) as IMessageChannel;

                // Do the work
                await HandleTvshows(channel, cancellationToken);
                await HandleMovies(channel, cancellationToken);
            }
        }

        async Task HandleTvshows(IMessageChannel channel, CancellationToken cancellationToken)
        {
            var msgs = await sonarrService.FindAllAsync();

            foreach (var msg in msgs)
            {
                try
                {
                    var embed = await sonarrService.CreateEmbed(msg);

                    if (embed != null)
                    {
                        await channel.TriggerTypingAsync();
                        await channel.SendMessageAsync(embed: embed);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to handle: {msg.series.title} - {msg.episodes?[0]?.title}");
                }
                finally
                {
                    await sonarrService.Delete(msg);
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        async Task HandleMovies(IMessageChannel channel, CancellationToken cancellationToken)
        {
            var msgs = await radarrService.FindAllAsync();

            foreach (var msg in msgs)
            {
                try
                {
                    var embed = await radarrService.CreateEmbed(msg);

                    if (embed != null)
                    {
                        await channel.TriggerTypingAsync();
                        await channel.SendMessageAsync(embed: embed);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Failed to handle: {msg.movie.title}");
                }
                finally
                {
                    await radarrService.Delete(msg);
                }

                await Task.Delay(1000, cancellationToken);
            }
        }

        async Task MessageReceived(SocketMessage rawMessage)
        {
            await commandHandlingService.MessageReceived(rawMessage, discord);
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Worker started at: {time}", DateTimeOffset.UtcNow);

            // Start discord bot
            await commandHandlingService.InitializeAsync();
            discord.MessageReceived += MessageReceived;
            await discord.LoginAsync(TokenType.Bot, options.Token);
            await discord.StartAsync();

            await base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Worker stopped at: {time}", DateTimeOffset.UtcNow);

            // Gracefully shut down discord bot
            discord.MessageReceived -= MessageReceived;
            await discord.LogoutAsync();
            await discord.StopAsync();
        }
    }
}
