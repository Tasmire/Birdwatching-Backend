using System.ComponentModel.DataAnnotations;

namespace Final_Project_Backend.Models
{
    public class RolesCreateViewModel
    {
        [Required]
        [StringLength(256, MinimumLength = 3)]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
