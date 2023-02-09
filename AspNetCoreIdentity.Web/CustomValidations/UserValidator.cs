using AspNetCoreIdentity.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.CustomValidations
{
    public class UserValidator : IUserValidator<AppUser>
    {
        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {

                var errors = new List<IdentityError>();
            var isNumeric = int.TryParse(user.UserName[0].ToString(), out _) ;


            if(isNumeric)
            {
                errors.Add(new IdentityError() { Code="UserNameContainsFirstLetterDigit"
                    ,Description="Kullanıcı adının ilk karakteri sayısal olamaz"});
            }



            if (errors.Any())//hata var
            {
                return Task.FromResult(IdentityResult.Failed(errors.ToArray()));
            }

            //hata yok 
            return Task.FromResult(IdentityResult.Success);
            

            
        }
    }
}
