using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using Craftmatrix.org.Model;
using Craftmatrix.Codex.org.Service;

namespace SIGLATAPI.Services
{
    public class AdminInitializationService : BackgroundService
    {
        private readonly ILogger<AdminInitializationService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public AdminInitializationService(ILogger<AdminInitializationService> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Admin Initialization Service started");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var db = scope.ServiceProvider.GetRequiredService<IPostgreService>();
                        await CheckAndCreateAdminUser(db);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while checking/creating admin user");
                }

                // Wait for 1 minute before next check
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }

        private async Task CheckAndCreateAdminUser(IPostgreService db)
        {
            try
            {
                var admins = await db.GetDataAsync<IdentityDto>("Identity");
                var admin = admins.Where(x => x.Role == "Admin");
                
                if (admin == null || !admin.Any())
                {
                    _logger.LogInformation("No admin user found. Creating default admin user...");
                    
                    var adminUser = new IdentityDto
                    {
                        Id = Guid.NewGuid(),
                        FirstName = "admin",
                        MiddleName = "",
                        LastName = "",
                        Address = "",
                        Gender = "",
                        PhoneNumber = "",
                        Role = "Admin",
                        DateOfBirth = DateTime.MinValue,
                        Email = "admin@gmail.com",
                        HashPass = PasswordService.HashPassword("123456"),
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    // Create a new object with DateOfBirth converted to DateTime to avoid Dapper DateOnly issues
                    var adminForDb = new
                    {
                        adminUser.Id,
                        adminUser.FirstName,
                        adminUser.MiddleName,
                        adminUser.LastName,
                        adminUser.Address,
                        adminUser.Role,
                        adminUser.DateOfBirth,
                        adminUser.Gender,
                        adminUser.PhoneNumber,
                        adminUser.Email,
                        adminUser.HashPass,
                        adminUser.CreatedAt,
                        adminUser.UpdatedAt
                    };

                    await db.PostDataAsync("Identity", adminForDb, adminUser.Id);
                    _logger.LogInformation("Default admin user created successfully with email: {Email}", adminUser.Email);
                }
                else
                {
                    _logger.LogDebug("Admin user already exists");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to check/create admin user");
                throw;
            }
        }
    }
}