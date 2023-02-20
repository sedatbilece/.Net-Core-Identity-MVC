using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentity.Web.Areas.Admin.Models;

namespace AspNetCoreIdentity.Web.Models
{
    public class AppDbContext : IdentityDbContext<AppUser,AppRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options){

        }
        public DbSet<AspNetCoreIdentity.Web.Areas.Admin.Models.RoleUpdateViewModel> RoleUpdateViewModel { get; set; } = default!;

    }
}
