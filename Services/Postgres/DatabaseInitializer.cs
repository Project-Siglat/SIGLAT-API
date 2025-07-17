using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Craftmatrix.org.Data;

public class DatabaseInitializer : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        IServiceProvider serviceProvider,
        ILogger<DatabaseInitializer> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting database initialization...");

        try
        {
            // Get PostgreService to ensure DB exists
            using var scope = _serviceProvider.CreateScope();
            var postgreService = scope.ServiceProvider.GetRequiredService<IPostgreService>();

            // Initialize database (ensure it exists)
            await postgreService.InitializeDatabaseAsync();

            // Apply EF Core migrations
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            _logger.LogInformation("Applying database migrations...");
            await dbContext.Database.MigrateAsync(cancellationToken);
            _logger.LogInformation("Database migrations applied successfully");

            // Test connection
            bool isConnected = await postgreService.CheckPing();
            if (isConnected)
            {
                _logger.LogInformation("Database connection confirmed");
            }
            else
            {
                _logger.LogWarning("Could not establish database connection");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during database initialization");
            // We're not rethrowing the exception as we want the application to continue
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
