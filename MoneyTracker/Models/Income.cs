using System.ComponentModel.DataAnnotations;

namespace MoneyTracker.Models
{
    public class Income
    {
        public int Id { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        public decimal Amount { get; set; }

        [Required]
        public string? Source { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
