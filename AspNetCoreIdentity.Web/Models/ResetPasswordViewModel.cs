using System.ComponentModel.DataAnnotations;


namespace AspNetCoreIdentity.Web.Models
{
    public class ResetPasswordViewModel
    {

        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        [Required(ErrorMessage = "Email boş olamaz.")]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }
}
