using Microsoft.EntityFrameworkCore;

namespace Imports.Data;

public class ImportsDbContext : DbContext
{
    public ImportsDbContext(DbContextOptions<ImportsDbContext> options)
    : base(options)
    {
        LoadImportedEvents = Set<SavedLoadImportedEvent>();
    }

    public DbSet<SavedLoadImportedEvent> LoadImportedEvents { get; set; }
}