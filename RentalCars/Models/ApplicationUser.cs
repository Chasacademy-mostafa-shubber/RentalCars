using Microsoft.AspNetCore.Identity;

namespace RentalCars.Models
{
    public class ApplicationUser:IdentityUser
    {
        public string FullName { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public bool IsAdmin { get; set; }
    }
}
