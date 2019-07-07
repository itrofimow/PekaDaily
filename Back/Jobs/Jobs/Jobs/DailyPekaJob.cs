using System;
using System.Threading.Tasks;
using Horarium.Interfaces;
using Jobs.Cockroach;
using Jobs.FCM;
using Jobs.Mongo;

namespace Jobs.Jobs
{
    public class DailyPekaJob : IJobRecurrent
    {
        private readonly IFcmSender _fcmSender;
        private readonly ICounterRepository _counterRepository;
        private readonly IPekaRepository _pekaRepository;
        private readonly ICockroachPekaRepository _cockroachPekaRepository;

        public DailyPekaJob(
            IFcmSender fcmSender,
            ICounterRepository counterRepository,
            IPekaRepository pekaRepository,
            ICockroachPekaRepository cockroachPekaRepository)
        {
            _fcmSender = fcmSender;
            _counterRepository = counterRepository;
            _pekaRepository = pekaRepository;
            _cockroachPekaRepository = cockroachPekaRepository;
        }
        
        public async Task Execute()
        {
            var counter = await _counterRepository.GetCurrent();
            if (!await _pekaRepository.CheckCounter(counter + 1))
                return;
            ++counter;

            await _counterRepository.IncrementCurrent();
            var newPeka = await _pekaRepository.GetPeka(counter);

            await _cockroachPekaRepository.SetPeka(newPeka.Url);

            await _fcmSender.SendNewPeka();
        }
    }
}