using System;
using System.Collections.Generic;
using System.IO;
using Horarium;
using Horarium.AspNetCore;
using Horarium.Interfaces;
using Horarium.Mongo;
using Jobs.Cockroach;
using Jobs.FCM;
using Jobs.Jobs;
using Jobs.Models;
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
            
            //LoadMongo(app.ApplicationServices.GetService<IPekaRepository>());
            StartJobs(app.ApplicationServices.GetService<IHorarium>());
        }

        private void StartJobs(IHorarium horarium)
        {
            horarium.CreateRecurrent<DailyPekaJob>(Cron.SecondInterval(10)).Schedule().Wait();
            //horarium.CreateRecurrent<DailyPekaJob>(Cron.Daily(13, 37)).Schedule().Wait();
        }

        /*private void LoadMongo(IPekaRepository pekaRepository)
        {
            var lines = File.ReadAllLines("/home/itrofimow/Desktop/376544903/tmp.txt");
            List<String> all = new List<string>
            {
                "1.png",
                "2.png"
            };
            
            Shuffle(lines);
            all.AddRange(lines);
            
            var pekas = new List<Peka>();
            for (int i = 0; i < all.Count; ++i)
                pekas.Add(new Peka
                {
                    Counter = i,
                    Url = all[i]
                });

            pekaRepository.AddPekas(pekas).Wait();
        }
        
        private static Random rng = new Random();  

        public static void Shuffle<T>(IList<T> list)  
        {  
            int n = list.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                T value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }*/
    }
}


