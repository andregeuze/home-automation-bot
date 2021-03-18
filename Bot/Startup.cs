using Bot.Services;
using Bot.Workers;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Bot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Options
            services
                .Configure<LiteDbOptions>(Configuration.GetSection(nameof(LiteDbOptions)))
                .Configure<OmdbServiceOptions>(Configuration.GetSection(nameof(OmdbServiceOptions)))
                .Configure<AutomationWorkerOptions>(Configuration.GetSection(nameof(AutomationWorkerOptions)))
                .Configure<RequestbinServiceOptions>(Configuration.GetSection(nameof(RequestbinServiceOptions)));

            // Http Clients
            services.AddHttpClient<OmdbService>(c => c.Timeout = TimeSpan.FromSeconds(5));
            services.AddHttpClient<RequestbinService>(c => c.Timeout = TimeSpan.FromSeconds(5));

            // Command service for Discord.Net
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();

            // Services
            services.AddSingleton<DatabaseService>();
            services.AddSingleton<OmdbService>();
            services.AddSingleton<SonarrService>();
            services.AddSingleton<RadarrService>();
            services.AddSingleton<RequestbinService>();
            services.AddSingleton<CommandHandlingService>();

            // Hosted services
            services.AddHostedService<AutomationWorker>();

            // API
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Bot API", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Bot API v1"));

            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
