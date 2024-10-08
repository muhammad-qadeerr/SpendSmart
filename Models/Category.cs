using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracker.Models
{
    public class Category
    {
        [Key]  // Indentifies Primary Key Attribute.
        public int CategoryId { get; set; }

        [Column(TypeName = "nvarchar(50)")]   //SQl Server Datatypes.
        public string  Title { get; set; }

        [Column(TypeName = "nvarchar(5)")]
        public string Icon { get; set; } = "";

        [Column(TypeName = "nvarchar(10)")]
        public string CategoryType { get; set; } = "Expense";
    }
}
