using Microsoft.AspNetCore.Identity;

namespace Final_Project_Backend.Models
{
    public class RolesIndexViewModel
    {
        public List<IdentityRole> Roles { get; set; }
        public List<UserRolesViewModel> UserRoles { get; set; }
    }

}
