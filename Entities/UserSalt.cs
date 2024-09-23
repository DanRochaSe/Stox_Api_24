using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Entities
{
    public class UserSalt
    {
        [Key]
        public int? UserSaltID { get; set; }

        [ForeignKey("UserName")]
        public string? UserName  { get; set; }

        public string? Salt { get; set; }


    }
}
