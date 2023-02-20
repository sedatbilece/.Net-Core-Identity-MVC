using System.ComponentModel.DataAnnotations;

namespace AspNetCoreIdentity.Web.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {

        [Display(Name="Role Adı")]
        public string Name { get; set; }
    }
}
