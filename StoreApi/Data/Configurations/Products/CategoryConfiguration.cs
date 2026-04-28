using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreApi.Models.Store;

namespace StoreApi.Data.Configurations.Store
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.ToTable(nameof(Category));
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name).HasMaxLength(100);

            builder.HasMany(c => c.Products)
                .WithOne(pr => pr.Category)
                .HasForeignKey(pr => pr.CategoryId);
        }
    }
}
