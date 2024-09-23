using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using webapi.Models;
namespace webapi.Entities
{
    public class User
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        public string? FirstName { get; set; }

        [Required]
        public string? LastName { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [EmailAddress(ErrorMessage = "Invalid Email format.")]
        public string? Email { get; set; }

        public string? PasswordHash { get; set; }

        public string Token { get; set; } = string.Empty;

        public string RefreshToken { get; set; } = string.Empty;

        public StoxEnum.RoleType Role { get; set; } 

        public Collection<Goals>? Goals { get; set; }
    }


    
}
