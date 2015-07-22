using CertiPay.Common.Notifications;
using RestSharp.Serializers;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace CertiPay.Notifications
{
    public class ServiceSender : INotificationSender<EmailNotification>, INotificationSender<SMSNotification>
    {
        public readonly Uri ServiceUri;

        public TimeSpan Timeout { get; set; }

        private readonly JsonSerializer Serializer = new JsonSerializer();

        public ServiceSender(Uri serviceUri)
        {
            this.ServiceUri = serviceUri;
            this.Timeout = TimeSpan.FromSeconds(3);
        }

        public ServiceSender(String serviceUri) : this(new Uri(serviceUri))
        {
            // Nothing to do here
        }

        public ServiceSender() : this("http://localhost:8081")
        {
            // Nothing to do here
        }

        public async Task SendAsync(SMSNotification notification)
        {
            await Post("/SMS", notification);
        }

        public async Task SendAsync(EmailNotification notification)
        {
            await Post("/Emails", notification);
        }

        public virtual async Task Post<T>(String resource, T t)
        {
            var json = Serializer.Serialize(t);

            var content = new StringContent(json, Encoding.Default, "application/json");

            await GetClient().PostAsync(resource, content);
        }

        public virtual HttpClient GetClient()
        {
            return new HttpClient() { BaseAddress = this.ServiceUri, Timeout = this.Timeout };
        }
    }
}