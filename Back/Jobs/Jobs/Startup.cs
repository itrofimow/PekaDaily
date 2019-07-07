using Horarium;
using Horarium.AspNetCore;
using Horarium.Interfaces;
using Horarium.Mongo;
using Jobs.FCM;
using Jobs.Jobs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Jobs
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddHorariumServer(MongoRepositoryFactory.Create("mongodb://localhost:27017/horarium"));
            services.AddSingleton<IFcmSender, FcmSender>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseMvc();
            app.ApplicationServices.StartHorariumServer();
            
            StartJobs(app.ApplicationServices.GetService<IHorarium>());
        }

        private void StartJobs(IHorarium horarium)
        {
            horarium.CreateRecurrent<DailyPekaJob>(Cron.Daily(13, 37)).Schedule().Wait();
        }
    }
}