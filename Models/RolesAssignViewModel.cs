using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Final_Project_Backend.Models
{
    public class RolesAssignViewModel
    {
        [Required(ErrorMessage = "Select a user")]
        public string UserId { get; set; } = null!;

        // store one-or-many selected role names
        [Required(ErrorMessage = "Select at least one role")]
        public List<string> SelectedRoles { get; set; } = new();

        // initialized to avoid nulls when redisplaying the view
        public IEnumerable<SelectListItem> Users { get; set; } = Enumerable.Empty<SelectListItem>();
        public IEnumerable<SelectListItem> Roles { get; set; } = Enumerable.Empty<SelectListItem>();
    }
}
