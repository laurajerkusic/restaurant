using Microsoft.EntityFrameworkCore;
using AbySalto.Junior.Models;

namespace AbySalto.Junior.Infrastructure.Database
{
    public class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // EF tablice:
        public DbSet<Order> Orders => Set<Order>();
        public DbSet<OrderItem> OrderItems => Set<OrderItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1:N – Order ima više OrderItem-a
            modelBuilder.Entity<Order>()
                .HasMany(o => o.Items)
                .WithOne(i => i.Order!)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Money kolona
            modelBuilder.Entity<OrderItem>()
                .Property(i => i.UnitPrice)
                .HasColumnType("decimal(18,2)");

            // Indeksi za učestale upite/sort
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.CreatedAt);
            modelBuilder.Entity<Order>()
                .HasIndex(o => o.Status);

            modelBuilder.Entity<Order>()
                .Property(o => o.Currency)
                .HasMaxLength(3)
                .HasDefaultValue("EUR");

            modelBuilder.Entity<Order>()
                .Property(o => o.PaymentMethod) // enum ide kao int po defaultu
                .HasConversion<int>();



        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
            => await base.SaveChangesAsync(cancellationToken);
    }
}
