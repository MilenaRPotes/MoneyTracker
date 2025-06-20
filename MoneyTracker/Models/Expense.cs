using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace MoneyTracker.Models
{
    public class Expense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string? Category { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
