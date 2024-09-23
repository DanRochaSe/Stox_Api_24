using webapi.Entities;

namespace webapi.Models
{
    public class GoalForReturnDTO
    {
        public int GoalID { get; set; }
        public DateTime DateAdded { get; set; }

        public DateTime TargetDate { get; set; }

        public string? Description { get; set; }

        public decimal TargetValue { get; set; }

        public int Period { get; set; }

        public Category Category { get; set; }
    }
}
