using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Entities
{
    public class StoxComparison
    {

        [Key]
        public int ComparisonID { get; set; }

        [ForeignKey("StoxID")]
        public int StoxID_1 { get; set; }

        [ForeignKey("StoxID")]
        public int StoxID_2 { get; set; }

        public string? Author { get; set; }

        [ForeignKey("UserID")]
        public int UserID { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string Comments { get; set; } = string.Empty;



    }
}
