using System.ComponentModel.DataAnnotations;

namespace webapi.Models
{
    public class PostForCreationDTO
    {
       
        [Required(ErrorMessage = "Title can not be longer than 50 characters.")]
        [MaxLength(50)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(50)]
        public string? Subject { get; set; }

        [Required]
        public string? Body { get; set; }


        public string? Tickers { get; set; }

        public string? Author { get; set; }

    }
}
