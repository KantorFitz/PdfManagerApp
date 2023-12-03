using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.Infrastructure.EntitiesConfigurations;

public class SearchResultEntityConfiguration : IEntityTypeConfiguration<SearchResult>
{
    public void Configure(EntityTypeBuilder<SearchResult> builder)
    {
        builder.ToTable("search_results");
        
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.FoundOnPage)
            .IsRequired();
        builder.Property(x => x.Sentence)
            .IsRequired();

        builder.HasOne(x => x.HistoricalBookDetail)
            .WithMany(x => x.SearchResults)
            .HasForeignKey(x => x.HistoricalBookDetailId);
    }
}
