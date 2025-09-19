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

        private static IQueryable<Order> ApplySort(IQueryable<Order> q, string? sort)
        {
            return sort switch
            {
                "total_asc" => q.OrderBy(o => o.Items.Sum(i => i.UnitPrice * i.Quantity)),
                "total_desc" => q.OrderByDescending(o => o.Items.Sum(i => i.UnitPrice * i.Quantity)),
                _ => q.OrderByDescending(o => o.Id)
            };
        }

        // LISTA (bez paginacije) 
        public async Task<List<OrderReadDto>> GetAllAsync(string? sort, CancellationToken ct)
        {
            var q = _db.Orders.AsNoTracking();
            q = ApplySort(q, sort);

            return await q
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

        // LISTA (paginirano)
        public async Task<PagedResult<OrderReadDto>> GetAllPagedAsync(int page, int pageSize, string? sort, CancellationToken ct)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 200) pageSize = 20;

            var q = _db.Orders.AsNoTracking();
            q = ApplySort(q, sort); 

            var total = await q.CountAsync(ct);

            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            return new PagedResult<OrderReadDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };
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
                        i.Id, i.ProductName, i.Quantity, i.UnitPrice, i.UnitPrice * i.Quantity
                    )).ToList()
                ))
                .SingleOrDefaultAsync(ct);

            return dto;
        }

        // KREIRAJ
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
