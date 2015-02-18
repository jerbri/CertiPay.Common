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

        public SmsService(String twilioAccountSid, String twilioAuthToken, String twilioSourceNumber)
        {
            this._twilioAccountSId = twilioAccountSid;
            this._twilioAuthToken = twilioAuthToken;
            this._twilioSourceNumber = twilioSourceNumber;
        }

        public Task SendAsync(SMSNotification notification)
        {
            // TODO Add logging

            // TODO Add error handling

            var client = new TwilioRestClient(_twilioAccountSId, _twilioAuthToken);

            foreach (var recipient in notification.Recipients)
            {
                client.SendSmsMessage(_twilioSourceNumber, recipient, notification.Content);
            }

            return Task.FromResult(0);
        }
    }
}