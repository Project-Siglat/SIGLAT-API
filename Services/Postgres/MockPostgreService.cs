using Microsoft.Extensions.Logging;

public class MockPostgreService : IPostgreService
{
    private readonly ILogger<MockPostgreService> _logger;

    public MockPostgreService(ILogger<MockPostgreService> logger)
    {
        _logger = logger;
        _logger.LogWarning("Using MockPostgreService - database functionality is disabled");
    }

    public Task<bool> CheckPing()
    {
        _logger.LogInformation("Mock database ping - returning true");
        return Task.FromResult(true);
    }

    public string GetConnectionString()
    {
        return "Host=localhost;Database=mock_db;Username=mock;Password=mock;Port=5432";
    }

    public Task InitializeDatabaseAsync()
    {
        _logger.LogInformation("Mock database initialization completed");
        return Task.CompletedTask;
    }

    public string GetDatabaseName()
    {
        return "mock_db";
    }

    public Task<IEnumerable<T>> GetDataAsync<T>(string tableName) where T : class
    {
        _logger.LogWarning($"Mock GetDataAsync called for table: {tableName}");
        return Task.FromResult(Enumerable.Empty<T>());
    }

    public Task<T> GetSingleDataAsync<T>(string tableName, object id) where T : class
    {
        _logger.LogWarning($"Mock GetSingleDataAsync called for table: {tableName}, id: {id}");
        return Task.FromResult<T>(null);
    }

    public Task<IEnumerable<T>> GetDataByColumnAsync<T>(string tableName, string column, object targetValue) where T : class
    {
        _logger.LogWarning($"Mock GetDataByColumnAsync called for table: {tableName}, column: {column}");
        return Task.FromResult(Enumerable.Empty<T>());
    }

    public Task<bool> PostDataAsync<T>(string tableName, T data, object id) where T : class
    {
        _logger.LogWarning($"Mock PostDataAsync called for table: {tableName}");
        return Task.FromResult(true);
    }

    public Task<bool> DeleteDataAsync(string tableName, object id)
    {
        _logger.LogWarning($"Mock DeleteDataAsync called for table: {tableName}, id: {id}");
        return Task.FromResult(true);
    }
}