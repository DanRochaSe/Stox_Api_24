using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class StoxForCreationDTO
    {

        public string AssetName { get; set; } = string.Empty;

        public string AssetTicket { get; set; } = string.Empty;

        public decimal OpeningPrice { get; set; }

        public decimal ClosingPrice { get; set; }

        public bool isInPortfolio { get; set; }

        public decimal Target { get; set; }

        public DateTime TargetDate { get; set; }
    }
}
