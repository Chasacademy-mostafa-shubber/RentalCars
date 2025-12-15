namespace RentalCar.Models
{
    public class RentalViewModel
    {
        public Car Car { get; set; }
        public DateTime StartDate  { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalPrice { get; set; }
    }
}
