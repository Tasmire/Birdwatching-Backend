using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Identity;

namespace Final_Project_Backend.Models
{
    public class Users : IdentityUser<Guid>
    {
        // Serialize Id as "UserId" for frontend compatibility
        [JsonPropertyName("UserId")]
        public override Guid Id
        {
            get => base.Id;
            set => base.Id = value;
        }

        // Serialize UserName as "Username"
        [JsonPropertyName("Username")]
        public override string? UserName
        {
            get => base.UserName;
            set => base.UserName = value;
        }

        // Serialize Email as "Email"
        [JsonPropertyName("Email")]
        public override string? Email
        {
            get => base.Email;
            set => base.Email = value;
        }

        // DO NOT expose PasswordHash to clients.
        [JsonIgnore]
        [Display(Name = "Password Hash")]
        public override string? PasswordHash
        {
            get => base.PasswordHash;
            set => base.PasswordHash = value;
        }

        // Application-specific extra field
        public string? DisplayName { get; set; }

        // Backwards-compatibility wrappers used throughout the server code (NOT mapped to EF, NOT serialized)
        [NotMapped]
        [JsonIgnore]
        public Guid UserId
        {
            get => Id;
            set => Id = value;
        }

        [NotMapped]
        [JsonIgnore]
        public string Username
        {
            get => UserName ?? string.Empty;
            set => UserName = value;
        }
    }
}
