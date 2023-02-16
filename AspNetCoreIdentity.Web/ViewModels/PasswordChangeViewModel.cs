using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class PasswordChangeViewModel
    {

        [Required(ErrorMessage = "Eski Şifre boş olamaz.")]
        [Display(Name = "Eski Şifre")]
        [MinLength(6 ,ErrorMessage ="Şifreniz en az 6 karakter olabilir")]
        public string PasswordOld { get; set; }


        [Required(ErrorMessage = "Yeni Şifre boş olamaz.")]
        [Display(Name = "Yeni Şifre")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string PasswordNew { get; set; }

        [Compare("PasswordNew", ErrorMessage = "Şifreler aynı değildir.")]
        [Required(ErrorMessage = "Yeni Şifre Tekrar boş olamaz.")]
        [Display(Name = "Yeni Şifre Tekrar")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olabilir")]
        public string PasswordConfirm { get; set; }
    }
}
