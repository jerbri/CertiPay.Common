using CertiPay.Common.Logging;
using System;
using System.Threading.Tasks;
using Twilio;

namespace CertiPay.Common.Notifications
{
    /// <summary>
    /// Send an SMS message to the given recipient.
    /// </summary>
    /// <remarks>
    /// Implementation may be sent into background processing.
    /// </remarks>
    public interface ISMSService : INotificationSender<SMSNotification>
    {
        // Task SendAsync(T notification);
    }

    public class SmsService : ISMSService
    {
        private static readonly ILog Log = LogManager.GetLogger<ISMSService>();

        private readonly String _twilioAccountSId;

        private readonly String _twilioAuthToken;

        private readonly String _twilioSourceNumber;

        public SmsService(TwilioConfig config)
        {
            this._twilioAccountSId = config.AccountSid;
            this._twilioAuthToken = config.AuthToken;
            this._twilioSourceNumber = config.SourceNumber;
        }

        public Task SendAsync(SMSNotification notification)
        {
            using (Log.Timer("SMSNotification.SendAsync"))
            {
                Log.Info("Sending SMSNotification {@Notification}", notification);

                // TODO Add error handling

                var client = new TwilioRestClient(_twilioAccountSId, _twilioAuthToken);

                foreach (var recipient in notification.Recipients)
                {
                    client.SendSmsMessage(_twilioSourceNumber, recipient, notification.Content);
                }

                return Task.FromResult(0);
            }
        }

        public class TwilioConfig
        {
            public String AccountSid { get; set; }

            public String AuthToken { get; set; }

            public String SourceNumber { get; set; }
        }
    }
}