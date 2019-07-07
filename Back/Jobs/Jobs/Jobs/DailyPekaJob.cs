using System.Threading.Tasks;
using Horarium.Interfaces;
using Jobs.FCM;

namespace Jobs.Jobs
{
    public class DailyPekaJob : IJobRecurrent
    {
        private readonly IFcmSender _fcmSender;

        public DailyPekaJob(IFcmSender fcmSender)
        {
            _fcmSender = fcmSender;
        }
        
        public Task Execute()
        {
            return _fcmSender.SendNewPeka();
        }
    }
}