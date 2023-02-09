using AspNetCoreIdentity.Web.CustomValidations;
using AspNetCoreIdentity.Web.Models;

namespace AspNetCoreIdentity.Web.Extensions
{
    public static class StartupExtensions
    {

        public static void AddIdentityExtension(this IServiceCollection services )
        {

           services.AddIdentity<AppUser, AppRole>(options =>
            {

                options.User.RequireUniqueEmail = true;
                options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvyzwx0123456789_";

                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;


            })
                .AddPasswordValidator<PasswordValidator>()
                .AddUserValidator<UserValidator>()
                .AddEntityFrameworkStores<AppDbContext>();

        }
    }
}
