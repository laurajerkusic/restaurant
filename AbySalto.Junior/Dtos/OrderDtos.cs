using System.ComponentModel.DataAnnotations;
using AbySalto.Junior.Models;

namespace AbySalto.Junior.Dtos;

// CREATE 
public record OrderItemCreateDto(
    [property: Required, StringLength(120)] string ProductName,
    [property: Range(1, int.MaxValue)] int Quantity,
    [property: Range(0, 1_000_000)] decimal UnitPrice
);

public record OrderCreateDto(
    [property: Required, StringLength(120)] string CustomerName,
    [property: Phone, StringLength(40)] string? Phone,
    [property: StringLength(200)] string? DeliveryAddress,
    [property: StringLength(500)] string? Note,
    [property: Required] PaymentMethod PaymentMethod,
    [property: Required, StringLength(3)] string Currency,
    [property: Required] List<OrderItemCreateDto> Items
);

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
public record OrderUpdateStatusDto([property: Required] OrderStatus Status);
