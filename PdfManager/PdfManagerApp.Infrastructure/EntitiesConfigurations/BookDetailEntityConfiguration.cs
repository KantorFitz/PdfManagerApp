using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.Infrastructure.EntitiesConfigurations;

public class BookDetailEntityConfiguration : IEntityTypeConfiguration<BookDetail>
{
    public void Configure(EntityTypeBuilder<BookDetail> builder)
    {
        builder.ToTable("book_details");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.NumberOfPages)
            .IsRequired();
        builder.Property(x => x.FileName)
            .IsRequired();
        builder.Property(x => x.Title)
            .IsRequired();
        builder.Property(x => x.FolderId)
            .IsRequired();

        builder.HasOne(x => x.Folder)
            .WithMany(x => x.BookDetails)
            .HasForeignKey(x => x.FolderId);
    }
}
