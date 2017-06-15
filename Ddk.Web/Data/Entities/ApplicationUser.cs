using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Ddk.Data.Entities
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public string Address { get; set; }

        public string City { get; set; }
    }
}
