using System.Diagnostics;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentalCar.Data;
using RentalCar.Models;

namespace RentalCar.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        [TempData]
        public DateTime  _StartDate { get; set; }
        [TempData]
        public DateTime   _EndDate { get; set; }

        public HomeController(ILogger<HomeController> logger, UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _logger = logger;
            _userManager = userManager;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public IActionResult Index(DateTime ? startdate, DateTime ? enddate)
           
        {

            var availableCar = new List<Car>();
            

            if (startdate > enddate || startdate==enddate || startdate==null || enddate==null)
            {
                return View();
               
            }


            if (startdate != null || enddate != null)
            {
                _StartDate = startdate.Value;
                _EndDate = enddate.Value;

            }

            var Car_CTX = _context.Cars.Where(c => !c.Rentals.Any(r =>
               r.EndDate > startdate && enddate > r.StartDate)).ToList();

           foreach(var item in Car_CTX)
            {
                availableCar.Add(new Car
                {
                    CarId = item.CarId,
                    Mark = item.Mark,
                    Model = item.Model,
                    Gear = item.Gear,
                    Year = item.Year,
                    ImgUrl = item.ImgUrl,
                    Price = (_EndDate - _StartDate).Days * item.Price
                });
            }

            

            if(availableCar.Count == 0)
            {
                ViewBag.Message = "No car is available";
                return View();
            }

            ViewBag.AvailableCar = availableCar;

            


            return View();
        }

       

        [HttpPost]
        public IActionResult RentCar(int id)
        {
            var userId = _userManager.GetUserId(User);
            var car = _context.Cars.FirstOrDefault(c=> c.CarId==id);
            var rental = new Rental
            {
                StartDate = _StartDate,
                EndDate = _EndDate,
                TotalPrice = (_EndDate - _StartDate).Days * car.Price,
                CarId = car.CarId,
                UserId = userId
            };

            _context.Rentals.Add(rental);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        public IActionResult Privacy()
        {
            return View();
        }


        

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
