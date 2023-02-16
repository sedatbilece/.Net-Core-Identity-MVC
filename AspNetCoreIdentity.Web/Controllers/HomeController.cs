using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.Services;
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

        private readonly EmailService _emailService;
        public HomeController(ILogger<HomeController> logger ,
            UserManager<AppUser> userManager,  SignInManager<AppUser> signInManager, EmailService emailService)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailService = emailService;
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
            if (!ModelState.IsValid)// validation error
            {
                return View();
            }

            //herhangi bir url'e gidilecek erişim isterse urli tutmak için 
            returnUrl = returnUrl ?? Url.Action("Index", "Member");



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


        //Not working
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel request)
        {

            var hasUser = await _userManager.FindByEmailAsync(request.Email);

            if(hasUser == null)
            {
                ModelState.AddModelError(string.Empty,"Bu Email adresine sahip kullanıcı bulunamamıştır");
                return View();
            }

            string passwordToken =await  _userManager.GeneratePasswordResetTokenAsync(hasUser);

            string passwordResetLink = 
                Url.Action("ResetPassword", "Home", new { userId = hasUser.Id, Token = passwordToken });


            //email service
            await _emailService.SendResetEmail(passwordResetLink,hasUser.Email);


            TempData["SuccessMessage"] = "Şifre sıfırlama linki, Email adresinize gönderilmiştir.";
            return RedirectToAction("ForgetPassword", "Home");
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}