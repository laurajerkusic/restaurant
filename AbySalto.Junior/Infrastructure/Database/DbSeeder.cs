using AbySalto.Junior.Models;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Infrastructure.Database;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext db)
    {
        
        db.Database.Migrate();

        if (db.Orders.Any()) return;

        var o1 = new Order
        {
            CustomerName = "Ana",
            Phone = "0912345678",
            DeliveryAddress = "Dubrovacka 1, Makarska",
            Note = "Bez luka",
            CreatedAt = DateTime.UtcNow.AddMinutes(-45),
            Status = OrderStatus.InProgress,
            PaymentMethod = PaymentMethod.Card,
            Currency = "EUR",
            Items =
            {
                new OrderItem { ProductName = "Pizza Margherita", Quantity = 2, UnitPrice = 7.50m },
                new OrderItem { ProductName = "Coca-Cola 0.5",   Quantity = 2, UnitPrice = 2.00m }
            }
        };

        var o2 = new Order
        {
            CustomerName = "Marko",
            Phone = "098111222",
            DeliveryAddress = "Vukovarska, Vrgorac",
            Note = null,
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            Status = OrderStatus.Completed,
            PaymentMethod = PaymentMethod.Cash,
            Currency = "EUR",
            Items =
            {
                new OrderItem { ProductName = "Burger", Quantity = 1, UnitPrice = 8.90m },
                new OrderItem { ProductName = "Pomfrit", Quantity = 1, UnitPrice = 3.20m }
            }
        };

        var o3 = new Order
        {
            CustomerName = "Ivana",
            Phone = "098111222",
            DeliveryAddress = "AG Matoša, Vrgorac",
            Note = null,
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            Status = OrderStatus.Completed,
            PaymentMethod = PaymentMethod.Cash,
            Currency = "EUR",
            Items =
            {
                new OrderItem { ProductName = "Cheeseburger", Quantity = 1, UnitPrice = 9.90m },
                new OrderItem { ProductName = "Pomfrit", Quantity = 2, UnitPrice = 3.20m },
                new OrderItem { ProductName = "Coca-Cola 0.5", Quantity = 2, UnitPrice = 2.00m }
            }
        };

        var o4 = new Order
        {
            CustomerName = "Dora",
            Phone = "098111222",
            DeliveryAddress = "Splitska, Ploče",
            Note = null,
            CreatedAt = DateTime.UtcNow.AddHours(-2),
            Status = OrderStatus.Completed,
            PaymentMethod = PaymentMethod.Cash,
            Currency = "EUR",
            Items =
            {
                new OrderItem { ProductName = "Muffin", Quantity = 1, UnitPrice = 9.90m },
             
            }
        };

        db.Orders.AddRange(o1, o2, o3, o4);
        db.SaveChanges();
    }
}
