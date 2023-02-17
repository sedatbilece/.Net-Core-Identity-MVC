using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AspNetCoreIdentity.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {

        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        
        public MemberController(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager= signInManager;
            _userManager= userManager;
        }

        public async Task<IActionResult> Index()
        {

            var currentUser =await  _userManager.FindByNameAsync(User.Identity.Name);

            var userViewModel = new UserViewModelForList()
            {
                Username=currentUser!.UserName,
                Email=currentUser!.Email,
                PhoneNumber=currentUser!.PhoneNumber

            };

            return View(userViewModel);
        }


        public async Task<IActionResult> LogOut()
        {
           await  _signInManager.SignOutAsync();

            return RedirectToAction("Index","Home");
        }


        [HttpGet]
        public  IActionResult PasswordChange()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PasswordChange(PasswordChangeViewModel changeRequest)
        {
            if (!ModelState.IsValid)// validation error
            {
                return View();
            }

            var currentUser = await _userManager.FindByNameAsync(User.Identity!.Name!);

            var checkOldPassword = await _userManager.CheckPasswordAsync(currentUser, changeRequest.PasswordOld);

            if (!checkOldPassword)
            {
                ModelState.AddModelError(string.Empty, "Eski Şifreniz yanlış.");
                return View();
            }
            
            var result = await _userManager
                .ChangePasswordAsync(currentUser,changeRequest.PasswordOld, changeRequest.PasswordNew);


            if(!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, " Şifreniz değiştirilemedi.");
            }

           await  _userManager.UpdateSecurityStampAsync(currentUser);
           await  _signInManager.SignOutAsync();
            await _signInManager.PasswordSignInAsync(currentUser, changeRequest.PasswordNew, true,false);

            TempData["SuccessMessage"] = "Şifreniz başarı ile değiştirilmiştir";

            return RedirectToAction("PasswordChange","Member");
        }

        public async Task<IActionResult> UserEdit() { 

            ViewBag.gender = new SelectList(Enum.GetNames(typeof(Gender)));

            var currentUser = await _userManager.FindByNameAsync(User.Identity.Name);

            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                City = currentUser.City,
                BirtDate = currentUser.BirtDate,
                Gender = currentUser.Gender
            };
            return View(userEditViewModel); 
        }

    }
}
