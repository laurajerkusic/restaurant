using System.Linq;
using AbySalto.Junior.Dtos;
using AbySalto.Junior.Infrastructure.Database;
using AbySalto.Junior.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace AbySalto.Junior.Application.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IApplicationDbContext _db;
        private readonly IMapper _mapper;

        public OrderService(IApplicationDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }


        private static IQueryable<Order> ApplySort(IQueryable<Order> q, string? sort)
        {
            if (sort == "total_asc")
            {
                return q.OrderBy(o => o.Items.Sum(i => i.UnitPrice * i.Quantity));
            }

            if (sort == "total_desc")
            {
                return q.OrderByDescending(o => o.Items.Sum(i => i.UnitPrice * i.Quantity));
            }

            return q.OrderByDescending(o => o.Id);
        }


        // LISTA (bez paginacije)
        public async Task<List<OrderReadDto>> GetAllAsync(string? sort, CancellationToken ct)
        {
            var q = _db.Orders.AsNoTracking();
            q = ApplySort(q, sort);

            var result = await q
                .ProjectTo<OrderReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return result;
        }

        // LISTA (paginirano) + opcionalni sort
        public async Task<PagedResult<OrderReadDto>> GetAllPagedAsync(
            int page,
            int pageSize,
            string? sort,
            CancellationToken ct)
                {
            // validacija ulaznih parametara
            if (page < 1)
            {
                page = 1;
            }

            if (pageSize < 1 || pageSize > 200)
            {
                pageSize = 20;
            }

            // upit prema bazi
            var q = _db.Orders.AsNoTracking();
            q = ApplySort(q, sort);

            // ukupan broj zapisa
            var total = await q.CountAsync(ct);

            // dohvat samo tražene stranice
            var items = await q
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<OrderReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync(ct);

            // vraćanje rezultata
            var result = new PagedResult<OrderReadDto>
            {
                Items = items,
                TotalCount = total,
                Page = page,
                PageSize = pageSize
            };

            return result;
        }


        // DETALJ po Id-u
        public async Task<OrderReadDto?> GetByIdAsync(int id, CancellationToken ct)
        {
            var query = _db.Orders.AsNoTracking();

            var filtered = query.Where(o => o.Id == id);

            var projected = filtered.ProjectTo<OrderReadDto>(_mapper.ConfigurationProvider);

            var dto = await projected.SingleOrDefaultAsync(ct);

            return dto;
        }


        // KREIRAJ
        public async Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct)
        {
           
            if (dto.Items == null || dto.Items.Count == 0)
            {
                throw new ArgumentException("Order must contain at least one item.");
            }

            // DTO -> Entity
            var order = _mapper.Map<Order>(dto);

            //default values
            if (order.CreatedAt == default)
            {
                order.CreatedAt = DateTime.UtcNow;
            }

            if (order.Status == default)
            {
                order.Status = OrderStatus.Pending;
            }


            //Spremi entitet u bazu
            _db.Orders.Add(order);
            await _db.SaveChangesAsync(ct);

            var created = await _db.Orders
                .AsNoTracking()
                .Where(o => o.Id == order.Id)
                .ProjectTo<OrderReadDto>(_mapper.ConfigurationProvider)
                .SingleAsync(ct);

            return created;
        }


        // PROMJENA STATUSA
        public async Task<bool> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct)
        {
            
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

           
            if (order == null)
            {
                return false;
            }

            order.Status = status;


            await _db.SaveChangesAsync(ct);

    
            return true;
        }


        // BRISANJE
        public async Task<bool> DeleteAsync(int id, CancellationToken ct)
        {
            
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.Id == id, ct);

        
            if (order == null)
            {
                return false;
            }

    
            _db.Orders.Remove(order);

          
            await _db.SaveChangesAsync(ct);

         
            return true;
        }

    }
}
