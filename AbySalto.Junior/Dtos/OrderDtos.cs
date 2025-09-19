using System.ComponentModel.DataAnnotations;
using AbySalto.Junior.Models;

namespace AbySalto.Junior.Dtos;

// CREATE 
public class OrderItemCreateDto
{
    [Required, StringLength(120)] public string ProductName { get; set; } = "";
    [Range(1, int.MaxValue)] public int Quantity { get; set; }
    [Range(0, 1_000_000)] public decimal UnitPrice { get; set; }
}

public class OrderCreateDto
{
    [Required, StringLength(120)] public string CustomerName { get; set; } = "";
    [Phone, StringLength(40)] public string? Phone { get; set; }
    [StringLength(200)] public string? DeliveryAddress { get; set; }
    [StringLength(500)] public string? Note { get; set; }
    [Required] public PaymentMethod PaymentMethod { get; set; }
    [Required, StringLength(3)] public string Currency { get; set; } = "EUR";
    [Required] public List<OrderItemCreateDto> Items { get; set; } = new();
}


//  READ 
public record OrderReadItemDto(
    int Id,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal LineTotal
);

public record OrderReadDto(
    int Id,
    string CustomerName,
    string? Phone,
    string? DeliveryAddress,
    string? Note,
    DateTime CreatedAt,
    OrderStatus Status,
    PaymentMethod PaymentMethod,
    string Currency,
    decimal Total,
    List<OrderReadItemDto> Items
);

// UPDATE STATUS 
public class OrderUpdateStatusDto
{
    [Required]
    public OrderStatus Status { get; set; }
}

