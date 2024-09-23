using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class PostDto
    {

        public int? PostID { get; set; }


        public string? Title { get; set; }

 
        public string? Subject { get; set; }

        public string? Body { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Tickers { get; set; }

 
        public string? Author { get; set; }

        public DateTime? UpdatedAt { get; set; }

    }
}
