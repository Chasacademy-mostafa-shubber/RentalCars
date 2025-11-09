using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RentalCars.Data;
using RentalCars.Models;
using Syncfusion.EJ2.Layouts;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;

namespace RentalCars.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _logger = logger;
        _context = context;
        _userManager = userManager;
        
    }


    public IActionResult Index()
    {
        return View();
    }


    
    [HttpPost]
    public IActionResult Index(ViewModel model)
    {
        var availableCars = new List<Car>();

        var carList = _context.Cars.ToList();
        var rentalList = _context.Rentals.ToList();

        if(model.StartTime==null && model.EndTme == null
             || model.StartTime==model.EndTme || model.StartTime>model.EndTme
            )
        {
            return RedirectToAction("Index");
        }

        

        if (rentalList.Count() > 0 && model.StartTime!=null && model.EndTme!=null && model.EndTme >model.StartTime)
        {
            foreach (var rentals in rentalList.Where(r=> model.StartTime>=r.RentalDate && model.EndTme<=r.ReturnDate))
            {
                foreach (var cars in carList.Where(c=> c.CarId!=rentals.CarId))
                {
                   availableCars.Add(cars);
                }
            }
        }

        if (rentalList.Count() == 0 && model.StartTime != null && model.EndTme != null && model.EndTme>model.StartTime)
        {
            
            availableCars = carList;
        }

        ViewBag.AvailableCars = availableCars;

        return View();
    }


    




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
