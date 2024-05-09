using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using SimpleProductAPI.Models;

namespace SimpleProductAPI.Context
{
    public interface IDataContext
    {
        public DbSet<Product> Products { get; set; }

        public DatabaseFacade GetDatabaseFacade();
        public Task<int> SaveChangesAsync();

        public DbSet<TEntity> Set<TEntity>() where TEntity : class;
    }

    public class DataContext : DbContext, IDataContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        public DatabaseFacade GetDatabaseFacade()
        {
            return this.Database;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Name).HasMaxLength(100).IsRequired();
                entity.Property(e => e.Description).HasMaxLength(-1).IsRequired();
                entity.Property(e => e.Price).HasPrecision(18, 2).IsRequired();
                entity.Property(e => e.Quantity).IsRequired();
                entity.Property(e => e.CreatedDate).ValueGeneratedOnAdd().IsRequired();
                entity.Property(e => e.ModifiedDate).ValueGeneratedOnUpdate();
                entity.ToTable("Products");
            });
        }

    }
}
