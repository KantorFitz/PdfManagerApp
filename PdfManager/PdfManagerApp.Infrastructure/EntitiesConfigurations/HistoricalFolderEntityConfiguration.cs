using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.Infrastructure.EntitiesConfigurations;

public class HistoricalFolderEntityConfiguration : IEntityTypeConfiguration<HistoricalFolder>
{
    public void Configure(EntityTypeBuilder<HistoricalFolder> builder)
    {
        builder.ToTable("historical_folders");

        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.AbsolutePath)
            .IsRequired();

        builder.HasOne(x => x.SearchLog)
            .WithMany(x => x.HistoricalFolders)
            .HasForeignKey(x => x.SearchLogId);

        builder.HasMany(x => x.HistoricalBookDetails)
            .WithOne(x => x.HistoricalFolder)
            .HasForeignKey(x => x.HistoricalFolderId);
    }
}
