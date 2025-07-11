using System.Threading.Tasks;

namespace FitnessTracking.Interfaces
{
 
    public interface INotificationService
    {
        Task SendNotificationAsync(string userId, string message, bool sendEmail = false);
    }
}