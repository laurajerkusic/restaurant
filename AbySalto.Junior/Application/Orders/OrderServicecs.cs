using System.Linq;
using AbySalto.Junior.Dtos;
using AbySalto.Junior.Infrastructure.Database;
using AbySalto.Junior.Models;
using Microsoft.EntityFrameworkCore;

namespace AbySalto.Junior.Application.Orders
{
   
    public class OrderService : IOrderService
    {
        private readonly IApplicationDbContext _db;

        public OrderService(IApplicationDbContext db)
        {
            _db = db;
        }

        // LISTA 
        public async Task<List<OrderReadDto>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.CreatedAt) // default sortiranje (možeš i maknuti)
                .Select(o => new OrderReadDto(
                    o.Id,
                    o.CustomerName,
                    o.Phone,
                    o.DeliveryAddress,
                    o.Note,
                    o.CreatedAt,
                    o.Status,
                    o.PaymentMethod,
                    o.Currency,
                    o.Items.Sum(i => i.UnitPrice * i.Quantity),
                    o.Items.Select(i => new OrderReadItemDto(
                        i.Id, i.ProductName, i.Quantity, i.UnitPrice, i.UnitPrice * i.Quantity
                    )).ToList()
                ))
                .ToListAsync(ct);
        }
        // DETALJ po Id-u
        public async Task<OrderReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var dto = await _db.Orders.AsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new OrderReadDto(
                    o.Id,
                    o.CustomerName,
                    o.Phone,
                    o.DeliveryAddress,
                    o.Note,
                    o.CreatedAt,
                    o.Status,
                    o.PaymentMethod,
                    o.Currency,
                    o.Items.Sum(i => i.UnitPrice * i.Quantity),
                    o.Items.Select(i => new OrderReadItemDto(
                        i.Id,
                        i.ProductName,
                        i.Quantity,
                        i.UnitPrice,
                        i.UnitPrice * i.Quantity
                    )).ToList()
                ))
                .SingleOrDefaultAsync(ct);

            return dto; 
        }

        // KREIRAJ novu narudžbu
        public async Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct)
        {
           
            if (dto.Items == null || dto.Items.Count == 0)
                throw new ArgumentException("Order must contain at least one item.");

          
            var order = new Order
            {
                CustomerName = dto.CustomerName,
                Phone = dto.Phone,
                DeliveryAddress = dto.DeliveryAddress,
                Note = dto.Note,
                PaymentMethod = dto.PaymentMethod,
                Currency = dto.Currency,
                Status = OrderStatus.Pending,
                CreatedAt = DateTime.UtcNow,
                Items = dto.Items.Select(i => new OrderItem
                {
                    ProductName = i.ProductName,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            };

            _db.Orders.Add(order);
            await _db.SaveChangesAsync(ct);

            // pročitaj natrag kao DTO 
            var created = await GetByIdAsync(order.Id, ct);
            return created!;
        }

        // PROMJENA STATUSA
        public async Task<bool> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
            if (order == null) return false;

            order.Status = status;
            await _db.SaveChangesAsync(ct);
            return true;
        }

        // BRISANJE
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);
            if (order == null) return false;

          
            _db.Orders.Remove(order);

            await _db.SaveChangesAsync(ct);
            return true;
        }
    }
}
