using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.Infrastructure.EntitiesConfigurations;

public class SearchLogEntityConfiguration : IEntityTypeConfiguration<SearchLog>
{
    public void Configure(EntityTypeBuilder<SearchLog> builder)
    {
        builder.ToTable("search_logs");
        
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .IsRequired();
        builder.Property(x => x.SearchFinishReason)
            .IsRequired();
        builder.Property(x => x.SeekedPhrasesJsonList)
            .IsRequired();

        builder.HasMany(x => x.HistoricalFolders)
            .WithOne(x => x.SearchLog)
            .HasForeignKey(x => x.SearchLogId);
    }
}
