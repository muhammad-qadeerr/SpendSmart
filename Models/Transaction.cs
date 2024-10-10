using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Amount shoud be greater than 0.")]
        public int Amount { get; set; }

        [Column(TypeName = "nvarchar(75)")]
        public string? Note { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        // Each transaction will have a CategoryId (FK) 

        [Range(1,int.MaxValue, ErrorMessage = "Please select a category.")]
        public int CategoryId { get; set; }
        public Category? Category { get; set; } // - Using Navigation Property -- passed nullable for validations

        [NotMapped]
        public string? CategoryTitleWithIcon
        {
            get
            {
                return Category == null? " ": Category.Icon + " " + Category.Title;
            }
        }

        [NotMapped]
        public string? FormattedAmount
        {
            get
            {
                return ((Category == null || Category.CategoryType == "Expense")? "-" : "+") + " " + Amount.ToString("C0");  // C0 -- Currency without decimal.
            }
        }


    }
}
