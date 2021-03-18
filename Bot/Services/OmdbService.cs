using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bot.Services
{
    public class OmdbServiceOptions
    {
        public string ApiKey { get; set; }
    }

    public class OmdbService
    {
        private readonly OmdbServiceOptions options;
        private readonly HttpClient httpClient;

        public OmdbService(
            IOptions<OmdbServiceOptions> options,
            HttpClient httpClient)
        {
            this.options = options.Value;
            this.httpClient = httpClient;
        }

        public async Task<OmdbMessage> Get(string imdbId, string title)
        {
            var url = $"http://www.omdbapi.com/?apikey={options.ApiKey}";

            if (!string.IsNullOrEmpty(imdbId))
                url += $"&i={imdbId}";
            else
                url += $"&t={title}";

            // Call OMDB
            var res = await httpClient.GetAsync(url);
            var str = await res.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<OmdbMessage>(str);
            if (!res.IsSuccessStatusCode || result.Response == "False")
                throw new ApplicationException($"IMDB details not found for id: '{imdbId}'");

            return result;
        }
    }

    public class OmdbMessage
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }
        public string Released { get; set; }
        public string Runtime { get; set; }
        public string Genre { get; set; }
        public string Director { get; set; }
        public string Writer { get; set; }
        public string Actors { get; set; }
        public string Plot { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public Rating[] Ratings { get; set; }
        public string Metascore { get; set; }
        public string imdbRating { get; set; }
        public string imdbVotes { get; set; }
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string totalSeasons { get; set; }
        public string Response { get; set; }
    }

    public class Rating
    {
        public string Source { get; set; }
        public string Value { get; set; }
    }
}
