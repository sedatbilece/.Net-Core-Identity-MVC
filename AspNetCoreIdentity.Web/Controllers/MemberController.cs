using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.FileProviders;

namespace AspNetCoreIdentity.Web.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {

        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;
        private readonly IFileProvider _fileProvider;
        
        public MemberController(SignInManager<AppUser> signInManager, 
            UserManager<AppUser> userManager, IFileProvider fileProvider)
        {
            _signInManager= signInManager;
            _userManager= userManager;
            _fileProvider = fileProvider;
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

        [HttpPost]
        public async Task<IActionResult> UserEdit(UserEditViewModel request)
        {

            if(!ModelState.IsValid)
            {
                return View();
            }

            var currentUser =await _userManager.FindByNameAsync(User.Identity!.Name!);

            currentUser.UserName = request.UserName;
            currentUser.Email = request.Email;
            currentUser.PhoneNumber = request.Phone;
            currentUser.BirtDate = request.BirtDate;
            currentUser.City = request.City;
            currentUser.Gender = request.Gender;




            if(request.Picture!= null && request.Picture.Length>0)
            {
                var wwwrootFolder = _fileProvider.GetDirectoryContents("wwwroot");

                var randomFileName = $"{Guid.NewGuid().ToString()}{Path.GetExtension(
                    request.Picture.FileName)}";


                var newPath = Path.Combine(wwwrootFolder.First(x => x.Name == "userpictures").PhysicalPath!,randomFileName);
                   using  var stream = new FileStream(newPath,FileMode.Create);   

                await request.Picture.CopyToAsync(stream);


                currentUser.Picture = randomFileName;
            }

           var updateUser= await _userManager.UpdateAsync(currentUser);

            if (!updateUser.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Düzenleme yapılamadı !");
            }

           await _userManager.UpdateSecurityStampAsync(currentUser);
           await _signInManager.SignOutAsync();
           await _signInManager.SignInAsync(currentUser, true);


            var userEditViewModel = new UserEditViewModel()
            {
                UserName = currentUser.UserName,
                Email = currentUser.Email,
                Phone = currentUser.PhoneNumber,
                City = currentUser.City,
                BirtDate = currentUser.BirtDate,
                Gender = currentUser.Gender
            };
            TempData["SuccessMessage"] = "Düzenleme başarı ile gerçekleştirilmiştir";
            return View(userEditViewModel);
        }

    }
}
