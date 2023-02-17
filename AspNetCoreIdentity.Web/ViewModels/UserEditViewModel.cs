using AspNetCoreIdentity.Web.Models;
using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.ViewModels
{
    public class UserEditViewModel
    {

        [Required(ErrorMessage = "Kullanıcı Adı boş olamaz.")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; } = null!;

        [EmailAddress(ErrorMessage = "Email formatı yanlıştır.")]
        [Required(ErrorMessage = "Email boş olamaz.")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [Required(ErrorMessage = "Telefon boş olamaz.")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Doğum tarihi")]
        public DateTime? BirtDate { get; set; } = null;

        
        [Display(Name = "Şehir")]
        public string? City { get; set; }

        [Display(Name = "Cinsiyet")]
        public byte? Gender { get; set; }

        [Display(Name = "Resim")]
        public IFormFile? Picture { get; set; }

    }
}
