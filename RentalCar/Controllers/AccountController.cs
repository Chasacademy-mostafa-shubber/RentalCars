using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentalCar.Data;
using RentalCar.Models;

namespace RentalCar.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        
        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UserProfile()
        {
            var userId = _userManager.GetUserId(User);
            var profile = _context.Profiles.FirstOrDefault(x=> x.UserId== userId);
            
            if(profile == null)
            {
                return RedirectToAction("CreateProfile");
            }

            return View(profile);
        }

        
        public IActionResult CreateProfile()
        {
            
            return View();
        }


        [HttpPost]
        public IActionResult CreateProfile(Profile model)
        {
            var userID = _userManager.GetUserId(User);
            _context.Profiles.Add(new Profile
            {
                FullName = model.FullName,
                Address = model.Address,
                City = model.City,
                ZipCode = model.ZipCode,
                Phone = model.Phone,
                UserId = userID

            });
            _context.SaveChanges();
            return RedirectToAction("UserProfile");
        }

        public IActionResult EditProfile()
        {
            var userID = _userManager.GetUserId(User);
            var profile = _context.Profiles.FirstOrDefault(x=> x.UserId==userID);
            return View(profile);
        }

        [HttpPost]
        public IActionResult EditProfile(Profile model)
        {
            var userID = _userManager.GetUserId(User);
            var profile = _context.Profiles.FirstOrDefault(x=> x.UserId==userID);
            profile.FullName = model.FullName;
            profile.Address = model.Address;
            profile.City = model.City;
            profile.ZipCode = model.ZipCode;
            profile.Phone = model.Phone;

            _context.SaveChanges();
            return RedirectToAction("UserProfile");
        }

        public IActionResult UserRental()
        {
            var userID = _userManager.GetUserId(User);
            var userRentalList = _context.Rentals.Include(x => x.Car).Where(x => x.UserId == userID).ToList();

            return View(userRentalList);
        }
    }
}
