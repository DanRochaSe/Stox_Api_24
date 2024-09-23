using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Entities
{
    public class Stox
    {
        [Key]
        public int StoxID { get; set; }
        
        [Required]
        public string AssetName { get; set; } = string.Empty;

        [Required]
        public string AssetTicket { get; set; } = string.Empty;

        [Required]
        public decimal OpeningPrice { get; set; }

        [Required]
        public decimal ClosingPrice { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }

        public DateTime UpdatedAt { get; set; }

        [Required]
        public string Author { get; set; } = string.Empty;

        [Required]
        [ForeignKey("UserId")]
        public int UserID { get; set; }

        public bool isInPortfolio { get; set; }

        public decimal Target { get; set; }

        public DateTime TargetDate { get; set; }



    }
}
