using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AbySalto.Junior.Models
{
    public class OrderItem
    {

        public int Id { get; set; }

        [Required, StringLength(120)]
        public string ProductName { get; set; } = string.Empty;

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Range(0, 1_000_000)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        // FK
        public int OrderId { get; set; }
        public Order? Order { get; set; }
    }
}
