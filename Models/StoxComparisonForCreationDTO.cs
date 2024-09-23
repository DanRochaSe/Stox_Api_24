using System.ComponentModel.DataAnnotations.Schema;

namespace webapi.Models
{
    public class StoxComparisonForCreationDTO
    {

        public int StoxID_1 { get; set; }

        public int StoxID_2 { get; set; }

        public string Comments { get; set; } = string.Empty;

    }
}
