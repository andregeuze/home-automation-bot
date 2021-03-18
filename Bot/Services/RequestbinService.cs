using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Bot.Services
{
    public class RequestbinServiceOptions
    {
        public string Url { get; set; }
    }

    public class RequestbinService
    {
        private readonly RequestbinServiceOptions options;
        private readonly HttpClient httpClient;

        public RequestbinService(
            IOptions<RequestbinServiceOptions> options,
            HttpClient httpClient)
        {
            this.options = options.Value;
            this.httpClient = httpClient;
        }

        public async Task Post(string payload)
        {
            var res = await httpClient.PostAsync(options.Url, new StringContent(payload));

            if (!res.IsSuccessStatusCode)
                throw new ApplicationException($"Failed to post to Requestbin");
        }
    }
}
