using AbySalto.Junior.Dtos;
using AbySalto.Junior.Models;

namespace AbySalto.Junior.Application.Orders;

public interface IOrderService
{
    Task<List<OrderReadDto>> GetAllAsync(CancellationToken ct);
    Task<OrderReadDto?> GetByIdAsync(int id, CancellationToken ct);
    Task<OrderReadDto> CreateAsync(OrderCreateDto dto, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int id, OrderStatus status, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);

    Task<PagedResult<OrderReadDto>> GetAllPagedAsync(int page, int pageSize, CancellationToken ct);
}
