using Bot.Data;
using Discord;
using System.Threading.Tasks;

namespace Bot.Services
{
    public class RadarrService : EntityContext<RadarrMessage>
    {
        private readonly OmdbService omdbService;

        public RadarrService(
            DatabaseService databaseService,
            OmdbService omdbService) : base(databaseService)
        {
            this.omdbService = omdbService;
        }

        public async Task<Embed> CreateEmbed(RadarrMessage message)
        {
            var movieDetails = await omdbService.Get(message.movie.imdbId, message.movie.title);

            var builder = new EmbedBuilder();
            builder.WithTitle($"{movieDetails.Title} ({movieDetails.Year})");
            builder.WithAuthor("Radarr Movie Download Completed", "https://github.com/Radarr/radarr.github.io/blob/master/img/logo-256.png?raw=true");
            builder.WithThumbnailUrl(movieDetails.Poster);
            builder.WithCurrentTimestamp();
            builder.WithColor(0xfca103);
            builder.WithDescription($"https://www.imdb.com/title/{movieDetails.imdbID}");

            builder.AddField("Quality", message.movieFile.quality, true);
            builder.AddField("Release Group", message.movieFile.releaseGroup, true);
            builder.AddField("Release Date", message.movie.releaseDate, true);
            builder.AddField("Original Scene Name", message.movieFile.sceneName ?? message.movieFile.relativePath);

            return builder.Build();
        }
    }
}
