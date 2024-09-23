using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class UserForAuthentication
    {
        public int UserID { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }
        public DateTime DOB { get; set; }

        public string? Email { get; set; }

    }
}
