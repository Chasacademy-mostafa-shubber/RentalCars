using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Shared;
using RentalCars.Data;
using RentalCars.Models;

namespace RentalCars.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _usermanager;
        private readonly SignInManager<ApplicationUser> _signInManager;
       

        public AccountController(ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _usermanager = userManager;
            _signInManager = signInManager;
        }


       public IActionResult Settings()
       {
            return View();
       }

        public async Task<IActionResult> EditProfile()
        {
            var user = await _usermanager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ApplicationUser
            {
                FullName = user.FullName,
                Address = user.Address,
                City = user.City,
                ZipCode = user.ZipCode,
                PhoneNumber = user.PhoneNumber,


            };

            return View(model);
            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProfile(ApplicationUser model)
        {
            var user = await _usermanager.GetUserAsync(User);

            if (user == null)
            {
                return NotFound();
            }

            user.FullName = model.FullName;
            user.Address = model.Address;
            user.City = model.City;
            user.ZipCode = model.ZipCode;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _usermanager.UpdateAsync(user);

            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                ViewData["Message"] = "Your profile has been updated.";
                
                
                
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public async Task<IActionResult> ChangePassword()
        {
            
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(PasswordViewModel model)
        {
            var user = await _usermanager.GetUserAsync(User);
            var changePasswordResult = await _usermanager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);

            if (user == null)
            {
                return NotFound();
            }

            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View();
            }

            if (changePasswordResult.Succeeded)
            {
               await _signInManager.RefreshSignInAsync(user);


               ViewData["Message"] = "Your password has been changed.";
            }
            
            return View();
        }


        public IActionResult DeleteAccount()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task <IActionResult> DeleteAccount(PasswordViewModel model)
        {
            var user = await _usermanager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_usermanager.GetUserId(User)}'.");
            }

            bool RequirePassword = await _usermanager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _usermanager.CheckPasswordAsync(user, model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Incorrect password.");
                    return View();
                }
            }

            var result = await _usermanager.DeleteAsync(user);
            
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }

            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
            
        }
    }
}
