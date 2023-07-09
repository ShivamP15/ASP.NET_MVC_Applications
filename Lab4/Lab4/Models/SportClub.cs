using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace Lab4.Models
{
    public class SportClub
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Display(Name = "Registration Number")]
        [Required]
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; }

        [DataType(DataType.Currency)]
        [Column(TypeName = "money")]
        public decimal Fee { get; set; }
    }
}
