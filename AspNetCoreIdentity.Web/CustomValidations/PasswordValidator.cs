using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.CustomValidations
{
    public class PasswordValidator : IPasswordValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string? password)
        {

            var errors = new List<IdentityError>();
           
            if(password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new() { Code = "PasswordContainsUserName", Description = "Şifre alanı kullanıcı adı içeremez" });
            }

            if (password.ToLower().StartsWith("1234"))
            {
                errors.Add(new() { Code = "PasswordContains1234", Description = "Şifre alanı ardışık sayı içeremez" });
            }

            if(errors.Any())//hata var
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray() ));
            }
            else//hata yok  
            {
                return Task.FromResult(IdentityResult.Success);
            }
        }
    }
}
