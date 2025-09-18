using System.ComponentModel.DataAnnotations;

namespace AbySalto.Junior.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required, StringLength(120)]
        public string CustomerName { get; set; } = string.Empty;

        [Phone, StringLength(40)]
        public string? Phone { get; set; }

        [StringLength(200)]
        public string? DeliveryAddress { get; set; }

        [StringLength(500)]
        public string? Note { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        [Required, StringLength(3)]
        public string Currency { get; set; } = "EUR";

        public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();

        // Iznos narudžbe – izračun (nije kolona u bazi)
        public decimal Total => Items.Sum(i => i.Quantity * i.UnitPrice);
    }
}
