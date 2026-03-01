using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreApi.Models.Store;

namespace StoreApi.Data.Configurations.Store
{
    public class ProductSeoConfiguration : IEntityTypeConfiguration<ProductSeo>
    {
        public void Configure(EntityTypeBuilder<ProductSeo> builder)
        {
            builder.ToTable(nameof(ProductSeo));
            builder.HasKey(s => s.Id);
            
            builder.Property(s => s.Id).ValueGeneratedNever();
            builder.Property(s => s.MetaTitle).HasMaxLength(100);

            builder.HasOne(s => s.Product)
                .WithOne(pr => pr.ProductSeo)
                .HasForeignKey<ProductSeo>(s => s.Id)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
