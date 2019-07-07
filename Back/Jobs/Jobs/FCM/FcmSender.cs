using System.Collections.Generic;
using System.Threading.Tasks;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace Jobs.FCM
{
    public interface IFcmSender
    {
        Task SendNewPeka();
    }
    
    public class FcmSender : IFcmSender
    {
        public FcmSender()
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile("pekadaily-firebase-adminsdk.json")
            });
        }
        
        public async Task SendNewPeka()
        {
            try
            {
                var data = new Dictionary<string, string>();
                AddRequiredDataProperties(data);

                var notification = new Notification
                {
                    Title = "New Peka is out!",
                    Body = ":peka:"
                };
                
                var message = new Message
                {
                    Notification = notification,
                    Data = data,
                    Topic = "all"
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch 
            {
            }
        }
        
        private void AddRequiredDataProperties(Dictionary<string, string> data)
        {
            data["click_action"] = "FLUTTER_NOTIFICATION_CLICK";
        }
    }
}