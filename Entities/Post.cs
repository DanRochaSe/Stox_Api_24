using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace webapi.Entities
{
    public class Post
    {
        [Key]
        public int? PostID { get; set; }

        [Required(ErrorMessage = "Title can not be longer than 50 characters.")]
        [MaxLength(50)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Subject { get; set; }

        [Required]
        public string? Body { get; set; }

        public DateTime? CreatedAt { get; set; }

        public string? Tickers { get; set; }

        [Required]
        public string? Author { get; set; }

        public DateTime? UpdatedAt { get; set; }




    }
}
