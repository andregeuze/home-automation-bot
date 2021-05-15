using Bot.Data;
using Discord;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bot.Services
{
    public class RadarrServiceOptions
    {
        public string Url { get; set; }
        public string ApiKey { get; set; }
    }

    public class RadarrService : EntityContext<RadarrMessage>
    {
        private readonly RadarrServiceOptions options;
        private readonly HttpClient httpClient;
        private readonly OmdbService omdbService;

        public RadarrService(
            IOptions<RadarrServiceOptions> options,
            HttpClient httpClient,
            DatabaseService databaseService,
            OmdbService omdbService) : base(databaseService)
        {
            this.options = options.Value;
            this.httpClient = httpClient;
            this.omdbService = omdbService;
        }

        public async Task<Movie[]> Lookup(string searchTerm)
        {
            var url = $"{options.Url}/api/v3/movie/lookup?term={searchTerm}&apikey={options.ApiKey}";
            var str = await httpClient.GetStringAsync(url);
            var obj = JsonConvert.DeserializeObject<Movie[]>(str);
            return obj;
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



    public class Movie
    {
        public string title { get; set; }
        public string originalTitle { get; set; }
        public Alternatetitle[] alternateTitles { get; set; }
        public int secondaryYearSourceId { get; set; }
        public string sortTitle { get; set; }
        public long sizeOnDisk { get; set; }
        public string status { get; set; }
        public string overview { get; set; }
        public DateTime inCinemas { get; set; }
        public DateTime physicalRelease { get; set; }
        public DateTime digitalRelease { get; set; }
        public Image[] images { get; set; }
        public string website { get; set; }
        public string remotePoster { get; set; }
        public int year { get; set; }
        public bool hasFile { get; set; }
        public string youTubeTrailerId { get; set; }
        public string studio { get; set; }
        public string path { get; set; }
        public int qualityProfileId { get; set; }
        public bool monitored { get; set; }
        public string minimumAvailability { get; set; }
        public bool isAvailable { get; set; }
        public string folderName { get; set; }
        public int runtime { get; set; }
        public string cleanTitle { get; set; }
        public string imdbId { get; set; }
        public int tmdbId { get; set; }
        public string titleSlug { get; set; }
        public string folder { get; set; }
        public string certification { get; set; }
        public string[] genres { get; set; }
        public object[] tags { get; set; }
        public DateTime added { get; set; }
        public Ratings ratings { get; set; }
        public Moviefile movieFile { get; set; }
        public Collection collection { get; set; }
        public int id { get; set; }
    }

    public class Ratings
    {
        public int votes { get; set; }
        public float value { get; set; }
    }

    public class Moviefile
    {
        public int movieId { get; set; }
        public string relativePath { get; set; }
        public string path { get; set; }
        public long size { get; set; }
        public DateTime dateAdded { get; set; }
        public string sceneName { get; set; }
        public int indexerFlags { get; set; }
        public Quality quality { get; set; }
        public Mediainfo mediaInfo { get; set; }
        public string originalFilePath { get; set; }
        public bool qualityCutoffNotMet { get; set; }
        public Language[] languages { get; set; }
        public string releaseGroup { get; set; }
        public string edition { get; set; }
        public int id { get; set; }
    }

    public class Quality
    {
        public Quality1 quality { get; set; }
        public Revision revision { get; set; }
    }

    public class Quality1
    {
        public int id { get; set; }
        public string name { get; set; }
        public string source { get; set; }
        public int resolution { get; set; }
        public string modifier { get; set; }
    }

    public class Revision
    {
        public int version { get; set; }
        public int real { get; set; }
        public bool isRepack { get; set; }
    }

    public class Mediainfo
    {
        public string audioAdditionalFeatures { get; set; }
        public int audioBitrate { get; set; }
        public float audioChannels { get; set; }
        public string audioCodec { get; set; }
        public string audioLanguages { get; set; }
        public int audioStreamCount { get; set; }
        public int videoBitDepth { get; set; }
        public int videoBitrate { get; set; }
        public string videoCodec { get; set; }
        public float videoFps { get; set; }
        public string resolution { get; set; }
        public string runTime { get; set; }
        public string scanType { get; set; }
        public string subtitles { get; set; }
    }

    public class Language
    {
        public int id { get; set; }
        public string name { get; set; }
    }

    public class Collection
    {
        public string name { get; set; }
        public int tmdbId { get; set; }
        public object[] images { get; set; }
    }

    public class Alternatetitle
    {
        public string sourceType { get; set; }
        public int movieId { get; set; }
        public string title { get; set; }
        public int sourceId { get; set; }
        public int votes { get; set; }
        public int voteCount { get; set; }
        public Language language { get; set; }
        public int id { get; set; }
    }

    public class Image
    {
        public string coverType { get; set; }
        public string url { get; set; }
        public string remoteUrl { get; set; }
    }

}
