using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace RentalCars.Models
{
    public class Car
    {
        public int CarId { get; set; }
        [Required]
        public string Mark { get; set; }
        [Required]
        public string Model { get; set; }
        [Required]
        public string Year { get; set; }
        [Required]
        public string Gear { get; set; }
        [Required]
        [Display(Name ="Image url")]
        public string ImgUrl { get; set; }
        [Required]
        public int Price { get; set; }
        public List<Rental> Rentals { get; set; }
    }
}
