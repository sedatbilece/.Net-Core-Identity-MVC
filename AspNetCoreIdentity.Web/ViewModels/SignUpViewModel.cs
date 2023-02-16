using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class SignUpViewModel
    {

        public SignUpViewModel()
        {

        }
        public SignUpViewModel(string userName, string email, string phone, string password)
        {
            UserName= userName;
            Email= email;
            Phone= phone;
            Password= password;
        }

        [Required(ErrorMessage ="Kullanıcı Adı boş olamaz.")]
        [Display(Name ="Kullanıcı Adı")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage ="Email formatı yanlıştır.")]
        [Required(ErrorMessage = "Email boş olamaz.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Telefon boş olamaz.")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Şifre boş olamaz.")]
        [Display(Name = "Şifre")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string Password { get; set; }

        [Compare("Password",ErrorMessage ="Şifreler aynı değildir.")]
        [Required(ErrorMessage = "Şifre Tekrar boş olamaz.")]
        [Display(Name = "Şifre Tekrar")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string PasswordConfirm { get; set; }
    }
}
