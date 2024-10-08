using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        // Each transaction will have a CategoryId (FK) 
        public int CategoryId { get; set; }
        public Category Category { get; set; } // - Using Navigation Property.

    }
}
