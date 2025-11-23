using Microsoft.EntityFrameworkCore;
using TechHive.AI.Support.Models;

namespace TechHive.AI.Support.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<FaqItem> FaqItems => Set<FaqItem>();
    public DbSet<StoreSetting> StoreSettings => Set<StoreSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed demo products
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = 1,
                Name = "iPhone 15 Pro",
                Description = "Latest Apple flagship smartphone.",
                Price = 4500,
                Stock = 5
            },
            new Product
            {
                Id = 2,
                Name = "Galaxy S24",
                Description = "High-end Samsung smartphone.",
                Price = 3800,
                Stock = 10
            }
        );

        // Seed FAQ items
        modelBuilder.Entity<FaqItem>().HasData(
            new FaqItem
            {
                Id = 1,
                Question = "What is the shipping time inside Sweden?",
                Answer = "Shipping usually takes between 2 to 5 business days within Sweden."
            },
            new FaqItem
            {
                Id = 2,
                Question = "What is your return policy?",
                Answer = "You can return products within 14 days of delivery if they are in original condition."
            }
        );

        // Seed store settings
        modelBuilder.Entity<StoreSetting>().HasData(
            new StoreSetting
            {
                Id = 1,
                Key = "StoreName",
                Value = "TechHive"
            },
            new StoreSetting
            {
                Id = 2,
                Key = "SupportEmail",
                Value = "support@techhive.com"
            }
        );
    }
}
