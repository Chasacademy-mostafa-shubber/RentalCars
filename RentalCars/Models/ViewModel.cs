using Microsoft.CodeAnalysis.Operations;

namespace RentalCars.Models
{
    public class ViewModel
    {
        public Car Car { get; set; }
        public Rental Rental { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTme { get; set; }
    }
}
