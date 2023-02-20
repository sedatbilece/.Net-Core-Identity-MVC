using AspNetCoreIdentity.Web.Areas.Admin.Models;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreIdentity.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class RolesController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;


        public RolesController(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {

           var roleList = await  _roleManager.Roles.Select(x => new RoleListViewModel()
            {
                Id = x.Id,
                Name = x.Name
            }).ToListAsync();
            return View(roleList);
        }

        [HttpPost]
      public async Task<IActionResult> RoleUpdate(RoleUpdateViewModel model)
        {

            var roleUpdate = await _roleManager.FindByIdAsync(model.Id);
            roleUpdate.Name = model.Name;
            if (roleUpdate == null)
            {
                throw new Exception("Böyle bir rol bulunamamıştır");
            }
            await _roleManager.UpdateAsync(roleUpdate);
            TempData["Success"] = "Rol güncelleme  başarı ile gerçekleştirilmiştir";

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> RoleUpdate(string  id)
        {

            var roleUpdate = await _roleManager.FindByIdAsync(id);

            if (roleUpdate == null)
            {
                throw new Exception("Böyle bir rol bulunamamıştır");
            }

            return View(new RoleUpdateViewModel()
            {
                Id=roleUpdate.Id,
                Name = roleUpdate.Name
            });
        }


        [HttpGet]
        public IActionResult RolCreate()
        {
            return View();
        }


        [HttpPost]
        public async  Task<IActionResult> RolCreate(RoleCreateViewModel model)
        {
            var result = await _roleManager.CreateAsync(new AppRole() { Name = model.Name });

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Rol oluşturulamadı");
                return View();
            }

            TempData["Success"] = "Rol başarıyla oluşturuldu";
            return RedirectToAction("Index");
        }
    }
}
