using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

public class PostgreService : IPostgreService
{
    private readonly string _connectionString;
    private readonly string _databaseName;
    private readonly ILogger<PostgreService> _logger;
    private bool _initialized = false;

    public PostgreService(ILogger<PostgreService> logger)
    {
        _logger = logger;

        try
        {
            // Retrieve DB connection parameters from environment variables
            string dbHost = GetEnvironmentVariableOrThrow("DB_HOST");
            string dbPort = GetEnvironmentVariableOrThrow("DB_PORT");
            string dbUser = GetEnvironmentVariableOrThrow("DB_USER");
            string dbPass = GetEnvironmentVariableOrThrow("DB_PASS");
            _databaseName = GetEnvironmentVariableOrThrow("DB_DB");

            // Build connection string
            _connectionString = $"Host={dbHost};Port={dbPort};Database={_databaseName};Username={dbUser};Password={dbPass};";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize PostgreSQL connection string");
            throw;
        }
    }

    /// <summary>
    /// Creates and opens a new connection to the database.
    /// </summary>
    /// <returns>An open NpgsqlConnection</returns>
    private async Task<IDbConnection> CreateConnectionAsync()
    {
        var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();
        return connection;
    }

    /// <summary>
    /// Checks if the PostgreSQL connection is active.
    /// </summary>
    /// <returns>True if connected successfully, otherwise false.</returns>
    public async Task<bool> CheckPing()
    {
        try
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                _logger.LogInformation("PostgreSQL connection successful");
                return true;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "PostgreSQL connection failed");
            return false;
        }
    }

    /// <summary>
    /// Gets the current connection string.
    /// </summary>
    /// <returns>The PostgreSQL connection string.</returns>
    public string GetConnectionString() => _connectionString;

    /// <summary>
    /// Gets the database name.
    /// </summary>
    public string GetDatabaseName() => _databaseName;

    /// <summary>
    /// Initializes the database and ensures it exists.
    /// </summary>
    public async Task InitializeDatabaseAsync()
    {
        if (_initialized) return;

        await EnsureDatabaseExistsAsync();
        _initialized = true;
        _logger.LogInformation("Database initialization completed");
    }

    /// <summary>
    /// Ensures that the database exists, creates it if it doesn't
    /// </summary>
    private async Task EnsureDatabaseExistsAsync()
    {
        // Connection string to the postgres database to check if our DB exists
        string serverConnectionString = _connectionString.Replace($"Database={_databaseName}", "Database=postgres");

        try
        {
            using (var connection = new NpgsqlConnection(serverConnectionString))
            {
                await connection.OpenAsync();

                // Check if database exists
                string checkDbQuery = $"SELECT 1 FROM pg_database WHERE datname = '{_databaseName}'";
                bool dbExists = false;

                using (var cmd = new NpgsqlCommand(checkDbQuery, connection))
                {
                    var result = await cmd.ExecuteScalarAsync();
                    dbExists = (result != null);
                }

                // Create database if it doesn't exist
                if (!dbExists)
                {
                    _logger.LogInformation($"Database '{_databaseName}' does not exist. Creating...");
                    string createDbQuery = $"CREATE DATABASE \"{_databaseName}\"";
                    using (var cmd = new NpgsqlCommand(createDbQuery, connection))
                    {
                        await cmd.ExecuteNonQueryAsync();
                    }
                    _logger.LogInformation($"Database '{_databaseName}' created successfully");
                }
                else
                {
                    _logger.LogInformation($"Database '{_databaseName}' already exists");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to create database '{_databaseName}'");
            throw;
        }
    }

    /// <summary>
    /// Retrieves all data from a specified table.
    /// </summary>
    /// <typeparam name="T">The type to map the data to</typeparam>
    /// <param name="tableName">The name of the table to query</param>
    /// <returns>Collection of mapped objects</returns>
    public async Task<IEnumerable<T>> GetDataAsync<T>(string tableName) where T : class
    {
        try
        {
            using (var connection = await CreateConnectionAsync())
            {
                string query = $"SELECT * FROM \"{tableName}\"";
                _logger.LogDebug($"Executing query: {query}");

                var result = await connection.QueryAsync<T>(query);
                _logger.LogInformation($"Retrieved {result.Count()} records from table '{tableName}'");
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving data from table '{tableName}'");
            throw;
        }
    }

    /// <summary>
    /// Retrieves a single record by ID from a specified table.
    /// </summary>
    /// <typeparam name="T">The type to map the data to</typeparam>
    /// <param name="tableName">The name of the table to query</param>
    /// <param name="id">The ID value to look for</param>
    /// <returns>Single mapped object or null if not found</returns>
    public async Task<T> GetSingleDataAsync<T>(string tableName, object id) where T : class
    {
        try
        {
            using (var connection = await CreateConnectionAsync())
            {
                string query = $"SELECT * FROM \"{tableName}\" WHERE \"Id\" = @Id::uuid";
                _logger.LogDebug($"Executing query: {query} with Id = {id}");

                var result = await connection.QuerySingleOrDefaultAsync<T>(query, new { Id = id });

                if (result == null)
                {
                    _logger.LogInformation($"No record found in table '{tableName}' with ID {id}");
                }
                else
                {
                    _logger.LogInformation($"Retrieved record from table '{tableName}' with ID {id}");
                }

                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving record from table '{tableName}' with ID {id}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves data filtered by a specific column value.
    /// </summary>
    /// <typeparam name="T">The type to map the data to</typeparam>
    /// <param name="tableName">The name of the table to query</param>
    /// <param name="column">The column name to filter on</param>
    /// <param name="targetValue">The value to search for</param>
    /// <returns>Collection of mapped objects matching the criteria</returns>
    public async Task<IEnumerable<T>> GetDataByColumnAsync<T>(string tableName, string column, object targetValue) where T : class
    {
        try
        {
            using (var connection = await CreateConnectionAsync())
            {
                string query = $"SELECT * FROM \"{tableName}\" WHERE \"{column}\" = @Value";
                _logger.LogDebug($"Executing query: {query} with {column} = {targetValue}");

                var result = await connection.QueryAsync<T>(query, new { Value = targetValue });
                _logger.LogInformation($"Retrieved {result.Count()} records from table '{tableName}' where {column} = {targetValue}");
                return result;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving data from table '{tableName}' where {column} = {targetValue}");
            throw;
        }
    }

    /// <summary>
    /// Creates or updates data in the specified table.
    /// </summary>
    /// <typeparam name="T">The type of data object</typeparam>
    /// <param name="tableName">The name of the table</param>
    /// <param name="data">The data object to save</param>
    /// <param name="id">The ID of the record (for updates)</param>
    /// <returns>True if operation was successful, false otherwise</returns>
    public async Task<bool> PostDataAsync<T>(string tableName, T data, object id) where T : class
    {
        try
        {
            using (var connection = await CreateConnectionAsync())
            {
                // First check if record exists
                string checkQuery = $"SELECT COUNT(*) FROM \"{tableName}\" WHERE \"Id\" = @Id";
                int count = await connection.ExecuteScalarAsync<int>(checkQuery, new { Id = id });

                if (count > 0)
                {
                    // Record exists, perform UPDATE
                    return await UpdateRecordAsync(connection, tableName, data, id);
                }
                else
                {
                    // Record doesn't exist, perform INSERT
                    return await InsertRecordAsync(connection, tableName, data);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error saving data to table '{tableName}'");
            throw;
        }
    }

    /// <summary>
    /// Inserts a new record into the specified table.
    /// </summary>
    private async Task<bool> InsertRecordAsync<T>(IDbConnection connection, string tableName, T data) where T : class
    {
        try
        {
            // Get properties of the object
            var properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.GetGetMethod() != null)
                .ToList();

            // Build the SQL INSERT statement
            StringBuilder columnList = new StringBuilder();
            StringBuilder valuesList = new StringBuilder();

            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];
                columnList.Append($"\"{prop.Name}\"");
                valuesList.Append($"@{prop.Name}");

                if (i < properties.Count - 1)
                {
                    columnList.Append(", ");
                    valuesList.Append(", ");
                }
            }

            string query = $"INSERT INTO \"{tableName}\" ({columnList}) VALUES ({valuesList})";
            _logger.LogDebug($"Executing query: {query}");

            int rowsAffected = await connection.ExecuteAsync(query, data);
            _logger.LogInformation($"Inserted new record into table '{tableName}'");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error inserting record into table '{tableName}'");
            throw;
        }
    }

    /// <summary>
    /// Updates an existing record in the specified table.
    /// </summary>
    private async Task<bool> UpdateRecordAsync<T>(IDbConnection connection, string tableName, T data, object id) where T : class
    {
        try
        {
            // Get properties of the object (excluding Id)
            var properties = typeof(T).GetProperties()
                .Where(p => p.CanRead && p.GetGetMethod() != null && p.Name != "Id")
                .ToList();

            // Build the SQL UPDATE statement
            StringBuilder setClause = new StringBuilder();

            for (int i = 0; i < properties.Count; i++)
            {
                var prop = properties[i];
                setClause.Append($"\"{prop.Name}\" = @{prop.Name}");

                if (i < properties.Count - 1)
                {
                    setClause.Append(", ");
                }
            }

            // Create dynamic parameters object with Id included
            var parameters = new DynamicParameters(data);
            parameters.Add("Id", id);

            string query = $"UPDATE \"{tableName}\" SET {setClause} WHERE \"Id\" = @Id";
            _logger.LogDebug($"Executing query: {query}");

            int rowsAffected = await connection.ExecuteAsync(query, parameters);
            _logger.LogInformation($"Updated record in table '{tableName}' with ID {id}");
            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating record in table '{tableName}' with ID {id}");
            throw;
        }
    }

    /// <summary>
    /// Deletes a record from the specified table by ID.
    /// </summary>
    /// <param name="tableName">The name of the table</param>
    /// <param name="id">The ID of the record to delete</param>
    /// <returns>True if deletion was successful, false if record not found</returns>
    public async Task<bool> DeleteDataAsync(string tableName, object id)
    {
        try
        {
            using (var connection = await CreateConnectionAsync())
            {
                string query = $"DELETE FROM \"{tableName}\" WHERE \"Id\" = @Id";
                _logger.LogDebug($"Executing query: {query} with Id = {id}");

                int rowsAffected = await connection.ExecuteAsync(query, new { Id = id });

                if (rowsAffected > 0)
                {
                    _logger.LogInformation($"Deleted record from table '{tableName}' with ID {id}");
                    return true;
                }
                else
                {
                    _logger.LogInformation($"No record found to delete in table '{tableName}' with ID {id}");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting record from table '{tableName}' with ID {id}");
            throw;
        }
    }

    private string GetEnvironmentVariableOrThrow(string name)
    {
        string value = Environment.GetEnvironmentVariable(name);
        if (string.IsNullOrEmpty(value))
        {
            throw new InvalidOperationException($"Environment variable '{name}' is not set or is empty.");
        }
        return value;
    }
}
