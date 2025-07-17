using System.Collections.Generic;
using System.Threading.Tasks;

public interface IPostgreService
{
    /// <summary>
    /// Checks if the PostgreSQL connection is active.
    /// </summary>
    /// <returns>True if connected successfully, otherwise false.</returns>
    Task<bool> CheckPing();

    /// <summary>
    /// Gets the current connection string.
    /// </summary>
    /// <returns>The PostgreSQL connection string.</returns>
    string GetConnectionString();

    /// <summary>
    /// Initializes the database and ensures it exists.
    /// </summary>
    Task InitializeDatabaseAsync();

    /// <summary>
    /// Gets the database name.
    /// </summary>
    string GetDatabaseName();

    /// <summary>
    /// Retrieves all data from a specified table.
    /// </summary>
    /// <typeparam name="T">The type to map the data to</typeparam>
    /// <param name="tableName">The name of the table to query</param>
    /// <returns>Collection of mapped objects</returns>
    Task<IEnumerable<T>> GetDataAsync<T>(string tableName) where T : class;

    /// <summary>
    /// Retrieves a single record by ID from a specified table.
    /// </summary>
    /// <typeparam name="T">The type to map the data to</typeparam>
    /// <param name="tableName">The name of the table to query</param>
    /// <param name="id">The ID value to look for</param>
    /// <returns>Single mapped object or null if not found</returns>
    Task<T> GetSingleDataAsync<T>(string tableName, object id) where T : class;

    /// <summary>
    /// Retrieves data filtered by a specific column value.
    /// </summary>
    /// <typeparam name="T">The type to map the data to</typeparam>
    /// <param name="tableName">The name of the table to query</param>
    /// <param name="column">The column name to filter on</param>
    /// <param name="targetValue">The value to search for</param>
    /// <returns>Collection of mapped objects matching the criteria</returns>
    Task<IEnumerable<T>> GetDataByColumnAsync<T>(string tableName, string column, object targetValue) where T : class;

    /// <summary>
    /// Creates or updates data in the specified table.
    /// </summary>
    /// <typeparam name="T">The type of data object</typeparam>
    /// <param name="tableName">The name of the table</param>
    /// <param name="data">The data object to save</param>
    /// <param name="id">The ID of the record (for updates)</param>
    /// <returns>True if operation was successful, false otherwise</returns>
    Task<bool> PostDataAsync<T>(string tableName, T data, object id) where T : class;

    /// <summary>
    /// Deletes a record from the specified table by ID.
    /// </summary>
    /// <param name="tableName">The name of the table</param>
    /// <param name="id">The ID of the record to delete</param>
    /// <returns>True if deletion was successful, false if record not found</returns>
    Task<bool> DeleteDataAsync(string tableName, object id);
}
