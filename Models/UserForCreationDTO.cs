using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using webapi.Entities;

namespace webapi.Models
{
    public class UserForCreationDTO
    {

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? UserName { get; set; }

        public DateTime DOB { get; set; }

        public string? Email { get; set; }

        public string? Password { get; set; }


    }
}
