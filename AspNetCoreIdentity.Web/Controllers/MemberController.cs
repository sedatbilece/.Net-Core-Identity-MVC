﻿using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
    }
}
