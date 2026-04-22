using Domain.Entities.Store;

namespace Infrastructure.Data.Configurations.Store
{
    public class TagConfiguration : IEntityTypeConfiguration<Tag>
    {
        public void Configure(EntityTypeBuilder<Tag> builder)
        {
            builder.ToTable(nameof(Tag));
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name).HasMaxLength(100);

            builder.HasMany(t => t.Products).WithMany(pr => pr.Tags);
        }
    }
}
