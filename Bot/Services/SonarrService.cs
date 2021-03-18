using Bot.Data;
using Discord;
using System.Threading.Tasks;

namespace Bot.Services
{
    public class SonarrService : EntityContext<SonarrMessage>
    {
        private readonly OmdbService omdbService;

        public SonarrService(
            DatabaseService databaseService,
            OmdbService omdbService) : base(databaseService)
        {
            this.omdbService = omdbService;
        }

        public async Task<Embed> CreateEmbed(SonarrMessage message)
        {
            var episode = message.episodes[0];
            var tvshowDetails = await omdbService.Get(message.series.imdbId, message.series.title);

            var builder = new EmbedBuilder();
            builder.WithTitle($"{episode.title} - {message.series.title} ({tvshowDetails.Year})");
            builder.WithAuthor("Sonarr Episode Download Completed", "https://raw.githubusercontent.com/Sonarr/Sonarr/phantom-develop/Logo/256.png");
            builder.WithThumbnailUrl(tvshowDetails.Poster);
            builder.WithCurrentTimestamp();
            builder.WithColor(0x2193b5);
            builder.WithDescription($"https://www.imdb.com/title/{tvshowDetails.imdbID}");

            builder.AddField("Quality", message.episodeFile.quality, true);
            builder.AddField("Release Group", message.episodeFile.releaseGroup, true);
            builder.AddField("Air Date", message.episodes[0].airDate, true);
            builder.AddField("Original Scene Name", message.episodeFile.sceneName);

            return builder.Build();
        }

    }
}
