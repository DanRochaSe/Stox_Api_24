using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Entities
{


    public enum Category
    {
        Mortgage,
        Vehicles,
        Vacation,
        Retirement,
        Savings
    }


    public class Goals
    {
        [Key]
        public int GoalID { get; set; }

        [Required]
        public string? Author { get; set; }

        [Required]
        [ForeignKey("UserID")]
        public int? UserID { get; set; }

        public DateTime DateAdded { get; set; }

        public DateTime TargetDate { get; set; }
        [MaxLength(100)]
        public string? Description { get; set; }

        public decimal TargetValue { get; set; }

        public int Period { get; set; }

        public Category Category { get; set; }
    }
}
