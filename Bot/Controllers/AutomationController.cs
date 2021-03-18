using Bot.Data;
using Bot.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Bot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutomationController : ControllerBase
    {
        private readonly SonarrService sonarrService;
        private readonly RadarrService radarrService;
        private readonly RequestbinService requestbinService;

        public AutomationController(
            SonarrService sonarrService,
            RadarrService radarrService,
            RequestbinService requestbinService)
        {
            this.sonarrService = sonarrService;
            this.radarrService = radarrService;
            this.requestbinService = requestbinService;
        }

        [HttpPost("tvshows")]
        public async Task PostTvshow([FromBody] SonarrMessage message)
        {
            await requestbinService.Post(JsonConvert.SerializeObject(message));
            await sonarrService.Upsert(message);
        }

        [HttpPost("movies")]
        public async Task PostMovie([FromBody] RadarrMessage message)
        {
            await requestbinService.Post(JsonConvert.SerializeObject(message));
            await radarrService.Upsert(message);
        }
    }
}
