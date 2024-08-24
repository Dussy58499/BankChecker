using Microsoft.AspNetCore.Identity;

namespace Repository.Data
{
    public class ApplicationUser : IdentityUser
    {
        public bool ReceiveDailyNotifications { get; set; }
    }
}
