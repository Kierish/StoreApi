using Domain.Entities.Store;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Configurations.Store
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable(nameof(Product));
            builder.HasKey(pr => pr.Id);

            builder.Property(pr => pr.Name).HasMaxLength(100);
            builder.Property(pr => pr.Price).HasPrecision(18, 2);

            builder.HasMany(pr => pr.Tags).WithMany(t => t.Products);

            builder
                .HasOne(pr => pr.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(pr => pr.CategoryId);

            builder.OwnsOne(
                p => p.MetaData,
                seoBuilder =>
                {
                    seoBuilder.Property(s => s.MetaTitle).HasMaxLength(100);
                }
            );
        }
    }
}
