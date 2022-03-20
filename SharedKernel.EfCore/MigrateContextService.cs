using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.EfCore;

public class MigrateContextService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public MigrateContextService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task MigrateAsync<T>(CancellationToken cancellationToken)
        where T : DbContext
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<T>();
        
        await context.Database.MigrateAsync(cancellationToken);
    }
}