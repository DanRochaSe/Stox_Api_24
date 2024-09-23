using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using webapi.Entities;

namespace webapi.Models
{
    public class GoalForCreationDTO
    {

        public DateTime DateAdded { get; set; }

        public DateTime TargetDate { get; set; }
  
        public string? Description { get; set; }

        public decimal TargetValue { get; set; }

        public int Period { get; set; }

        public Category Category { get; set; }


    }
}
