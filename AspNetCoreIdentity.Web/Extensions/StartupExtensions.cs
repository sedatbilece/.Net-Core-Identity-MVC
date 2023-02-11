using AspNetCoreIdentity.Web.CustomValidations;
using AspNetCoreIdentity.Web.Localizations;
using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Extensions
{
    public static class StartupExtensions
    {

        public static void AddIdentityExtension(this IServiceCollection services )
        {


            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(2);
            });

            services.AddIdentity<AppUser, AppRole>(options =>
             {

                 options.User.RequireUniqueEmail = true;
                 options.User.AllowedUserNameCharacters = "abcdefghijklmnoprstuvyzwx0123456789_";

                 options.Password.RequiredLength = 6;
                 options.Password.RequireNonAlphanumeric = false;
                 options.Password.RequireLowercase = true;
                 options.Password.RequireUppercase = false;
                 options.Password.RequireDigit = false;


                 options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                 options.Lockout.MaxFailedAccessAttempts = 3;


             })
                 .AddPasswordValidator<PasswordValidator>()
                 .AddUserValidator<UserValidator>()
                 .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
                 .AddDefaultTokenProviders()
                 .AddEntityFrameworkStores<AppDbContext>();
                 

        }
    }
}
