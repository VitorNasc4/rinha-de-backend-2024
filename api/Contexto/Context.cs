using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Contexto;

public class Context : DbContext
{
    public DbSet<Cliente>? Clientes { get; set; }
    public DbSet<Transacao>? Transacoes { get; set; }
    public Context(DbContextOptions<Context> options) : base(options)
    {
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        AddTimestamp();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
    }

    private void AddTimestamp()
    {
        var entries = ChangeTracker.Entries().Where(e => e.Entity is BaseModel && (e.State == EntityState.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((BaseModel)entityEntry.Entity).UpdatedAt = DateTime.UtcNow;
            if (entityEntry.State == EntityState.Added)
            {
                ((BaseModel)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
            }
        }
    }
}