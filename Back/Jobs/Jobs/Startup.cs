using Horarium;
using Horarium.AspNetCore;
using Horarium.Interfaces;
using Horarium.Mongo;
using Jobs.Cockroach;
using Jobs.FCM;
using Jobs.Jobs;
using Jobs.Mongo;
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            
            services.AddHorariumServer(MongoRepositoryFactory.Create("mongodb://localhost:27017/horarium"));

            services.AddSingleton(_ => new MongoContext("mongodb://localhost:27017/peka_db"));
            
            services.AddSingleton<IPekaRepository, PekaRepository>();
            services.AddSingleton<ICockroachPekaRepository, CockroachPekaRepository>();
            services.AddSingleton<ICounterRepository, CounterRepository>();
            
            services.AddSingleton<IFcmSender, FcmSender>();
            services.AddSingleton<DailyPekaJob>();
        }

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