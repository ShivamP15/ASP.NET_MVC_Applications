using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace Lab4.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("SportClub Id")]
        [StringLength(50)]
        public string SportClubId { get; set; }

        [Required]
        [DisplayName("File Name")]
        [StringLength(100)]
        public string FileName { get; set; }

        [Required]
        [DisplayName("Image")]
        [StringLength(100)]
        [Url]
        public string Url { get; set; }
    }
}
