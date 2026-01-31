using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.Services;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Security.Claims;

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


        // Google ile giriş başlatma
        [HttpPost]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Home", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }

        // Google'dan geri dönüş
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Action("Index", "Member");

            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Harici sağlayıcı hatası: {remoteError}");
                return View("SignIn");
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(string.Empty, "Harici giriş bilgisi alınamadı.");
                return View("SignIn");
            }

            // Harici sağlayıcı ile giriş yapmayı dene
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);

            if (result.Succeeded)
            {
                return Redirect(returnUrl);
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(string.Empty, "Hesabınız kilitlendi.");
                return View("SignIn");
            }

            // Kullanıcı yoksa yeni hesap oluştur
            var email = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Email);
            var name = info.Principal.FindFirstValue(System.Security.Claims.ClaimTypes.Name);

            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                {
                    // Yeni kullanıcı oluştur
                    user = new AppUser
                    {
                        UserName = email.Split('@')[0],
                        Email = email
                    };

                    var createResult = await _userManager.CreateAsync(user);

                    if (!createResult.Succeeded)
                    {
                        foreach (var error in createResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View("SignIn");
                    }
                }

                // Harici giriş bilgisini kullanıcıya bağla
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);

                return Redirect(returnUrl);
            }

            ModelState.AddModelError(string.Empty, "Email bilgisi alınamadı.");
            return View("SignIn");
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