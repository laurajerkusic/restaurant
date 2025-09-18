using Microsoft.EntityFrameworkCore;
using AbySalto.Junior.Models;

namespace AbySalto.Junior.Infrastructure.Database
{
    public interface IApplicationDbContext
    {
        DbSet<Order> Orders { get; }
        DbSet<OrderItem> OrderItems { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
