using Microsoft.AspNetCore.Identity;

namespace Final_Project_Backend.Models
{
    public class UserRolesViewModel
    {
        public string UserName { get; set; }
        public List<string> Roles { get; set; } = new();
        public string? RoleName { get; set; }
        public bool HasRoles => Roles?.Any() == true;
        public int RoleCount => Roles?.Count ?? 0;
        public string RolesDisplay => Roles?.Any() == true ? string.Join(", ", Roles) : "No roles assigned";

    }
}
