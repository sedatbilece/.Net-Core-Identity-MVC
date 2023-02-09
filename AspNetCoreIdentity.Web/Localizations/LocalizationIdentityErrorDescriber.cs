using Microsoft.AspNetCore.Identity;

namespace AspNetCoreIdentity.Web.Localizations
{
    public class LocalizationIdentityErrorDescriber: IdentityErrorDescriber
    {


        public override IdentityError DuplicateUserName(string userName)
        {

            return new IdentityError { Code= "DuplicateUserName" ,Description=$" '{userName}'  başka bir kullanıcı tarafından kullanılmaktadır."};
            //return base.DuplicateUserName(userName);
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new IdentityError { Code = "DuplicateEmail", Description = $" '{email}'  başka bir kullanıcı tarafından kullanılmaktadır." };
        }


        public override IdentityError PasswordTooShort(int length)
        {
            return new IdentityError { Code = "PasswordTooShort", Description = $" Şifre en az {length} karakter olmalıdır" };
        }

    }
}
