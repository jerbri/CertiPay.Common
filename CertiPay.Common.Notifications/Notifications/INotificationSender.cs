using System.Threading.Tasks;

namespace CertiPay.Common.Notifications
{
    public interface INotificationSender<in T>
    {
        /// <summary>
        /// Send a notification asyncronously
        /// </summary>
        Task SendAsync(T notification);
    }
}