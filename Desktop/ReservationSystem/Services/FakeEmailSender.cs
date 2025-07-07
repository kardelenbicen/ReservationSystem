using Microsoft.AspNetCore.Identity.UI.Services;
using System.Threading.Tasks;

namespace ReservationSystem.Services
{
    public class FakeEmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Gerçek e-posta gönderimi yok, sadece tamamlandı olarak işaretle
            return Task.CompletedTask;
        }
    }
} 