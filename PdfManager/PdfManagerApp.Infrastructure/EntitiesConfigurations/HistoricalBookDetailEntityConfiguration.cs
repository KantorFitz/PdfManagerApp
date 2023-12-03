using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.Infrastructure.EntitiesConfigurations;

public class HistoricalBookDetailEntityConfiguration : IEntityTypeConfiguration<HistoricalBookDetail>
{
    public void Configure(EntityTypeBuilder<HistoricalBookDetail> builder)
    {
        builder.ToTable("historical_book_details");

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.FileNameWithExtension)
            .IsRequired();
        builder.Property(x => x.NumberOfPages)
            .IsRequired();

        builder.HasOne(x => x.HistoricalFolder)
            .WithMany(x => x.HistoricalBookDetails)
            .HasForeignKey(x => x.HistoricalFolderId);

        builder.HasMany(x => x.SearchResults)
            .WithOne(x => x.HistoricalBookDetail)
            .HasForeignKey(x => x.HistoricalBookDetailId);
    }
}
