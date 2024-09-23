using System.ComponentModel.DataAnnotations.Schema;
using webapi.Models;

namespace webapi.Entities
{
    public class UserRoles
    {
        [ForeignKey("UserID")]
        public int UserId { get; set; }

        public StoxEnum.RoleType Role { get; set; }

    }
}
