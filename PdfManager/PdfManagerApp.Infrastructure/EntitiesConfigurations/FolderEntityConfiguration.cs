using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.Infrastructure.EntitiesConfigurations;

public class FolderEntityConfiguration : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.ToTable("folders");

        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder.Property(x => x.AbsolutePath)
            .IsRequired();
        builder.Property(x => x.PdfAmount)
            .IsRequired();

        builder.HasMany(x => x.BookDetails)
            .WithOne(x => x.Folder)
            .HasForeignKey(x => x.FolderId);
    }
}