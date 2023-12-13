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
        optionsBuilder.UseLazyLoadingProxies(); // TODO[2023-12-03 17:01:56]: Consider dropping this if any performance loss will occur
        base.OnConfiguring(optionsBuilder);
    }

    public virtual DbSet<Folder> Folders { get; set; }
    public virtual DbSet<BookDetail> BookDetails { get; set; }
    public virtual DbSet<SearchLog> SearchLogs { get; set; }
    public virtual DbSet<HistoricalFolder> HistoricalFolders { get; set; }
    public virtual DbSet<HistoricalBookDetail> HistoricalBookDetails { get; set; }
    public virtual DbSet<SearchResult> SearchResults { get; set; }
}
