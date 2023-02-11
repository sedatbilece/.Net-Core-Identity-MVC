using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace AspNetCoreIdentity.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public HomeController(ILogger<HomeController> logger ,
            UserManager<AppUser> userManager,  SignInManager<AppUser> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        public IActionResult SignIn()
        {
            return View();
        }
        public IActionResult SignUp()
        {

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel model,string? returnUrl=null)
        {

            //herhangi bir url'e gidilecek erişim isterse urli tutmak için 
            returnUrl = returnUrl ?? Url.Action("Index", "Home");



            //email üzerinden userı alıyoruz
            var isUser = await _userManager.FindByEmailAsync(model.Email);

            if (isUser == null)
            {
                ModelState.AddModelError(string.Empty, "Email veya Şifre yanlış.");
                return View();
            }

            //login işlemi
            var result = await _signInManager.PasswordSignInAsync(isUser, model.Password,model.RememberMe,true);

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
                 
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "3 dk boyunca giriş yapamazsınız.");
                return View();
            }

            ModelState.AddModelError(string.Empty, $"Email veya Şifre yanlış [Başarısız giriş sayısı : " +
                $"{await _userManager.GetAccessFailedCountAsync(isUser)}]");

            return View();
        }

        
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel request)
        {

            if (!ModelState.IsValid)// validation error
            {
                return View();
            }


            //signup
            var identityResult = await   _userManager.CreateAsync(new()
            {
                UserName = request.UserName,
                Email = request.Email,
                PhoneNumber = request.Phone
            }, request.PasswordConfirm);


            

            if (identityResult.Succeeded)// signup redirect
            {
                TempData["SuccessMessage"] = "Kayıt işlemi başarı ile gerçekleştirilmiştir";
                return RedirectToAction(nameof(HomeController.SignUp));
            }
            

            //
            foreach(IdentityError item in identityResult.Errors)//Identity model errors
            {
                ModelState.AddModelError(string.Empty, item.Description);
            }
            return View();
        }



        public IActionResult ForgetPassword()
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