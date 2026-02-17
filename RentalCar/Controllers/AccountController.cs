using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using RentalCar.Data;
using RentalCar.Models;

namespace RentalCar.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(ApplicationDbContext context, UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
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

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task <IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new IdentityUser
            {
                UserName = model.Email,
                Email = model.Email,
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Account");

            ModelState.AddModelError("", "Wrong email or password");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

    }
}
