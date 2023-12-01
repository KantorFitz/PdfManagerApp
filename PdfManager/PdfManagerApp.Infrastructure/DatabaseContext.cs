using Microsoft.EntityFrameworkCore;
using PdfManagerApp.Domain.Entities;

namespace PdfManagerApp.Infrastructure;

public class DatabaseContext : DbContext
{
    public DatabaseContext()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DatabaseContext).Assembly);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = Configuration.db");
        optionsBuilder.UseLazyLoadingProxies();
        base.OnConfiguring(optionsBuilder);
    }

    public virtual DbSet<Folder> Folders { get; set; }
    public virtual DbSet<BookDetail> BookDetails { get; set; }
}
