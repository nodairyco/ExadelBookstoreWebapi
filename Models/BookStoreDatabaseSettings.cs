namespace ExadelBookstoreAPI.Models;

/// <summary>
/// Class to connect to setup MongoDB database connection
/// </summary>
public class BookStoreDatabaseSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string BooksCollectionName { get; set; } = null!;
}