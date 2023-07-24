using System.ComponentModel.DataAnnotations;

namespace Lab5.Models
{
    public enum Question
    {
        Computer,
        Earth
    }
    public class Prediction
    {
        public int PredictionId { get; set; }
        
        [Required]
        public string FileName { get; set; }

        [Required]
        [Url]
        public string Url { get; set; }

        [Required]
        public Question Question { get; set; }
    }
}
