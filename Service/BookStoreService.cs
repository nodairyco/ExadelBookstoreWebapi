using ExadelBookstoreAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExadelBookstoreAPI.Service;

public class BookStoreService
{
    /// <summary>
    /// MongoDB repository.
    /// </summary>
    private readonly IMongoCollection<Book> _bookCollection;

    public BookStoreService(IOptions<BookStoreDatabaseSettings> databaseSettings)
    {
        var client = new MongoClient(databaseSettings.Value.ConnectionString);
        var database = client.GetDatabase(databaseSettings.Value.DatabaseName);
        _bookCollection = database.GetCollection<Book>(databaseSettings.Value.BooksCollectionName);
    }

    public async Task<List<Book>> GetAllPaginatedAsync(int page = 1, int pageSize = 10) 
        => await _bookCollection
            .Find(b => !b.isDeleted)
            .Skip((page - 1) * pageSize)
            .Limit(pageSize)
            .ToListAsync();

    public async Task<Book?> GetByTitleAsync(string title)
        => await _bookCollection.Find(b => b.title == title).FirstOrDefaultAsync();

    public async Task CreateBookAsync(Book book)
        => await _bookCollection.InsertOneAsync(book);

    public async Task UpdateByTitleAsync(string title, Book book)
        => await _bookCollection.ReplaceOneAsync(b => b.title == title, book);

    public async Task DeleteByTitleAsync(string title)
        => await _bookCollection.UpdateOneAsync(
            b => b.title == title, 
            Builders<Book>.Update.Set(b => b.isDeleted, true));

    public async Task UpdateViewCountByTitleAsync(string title)
        => await _bookCollection.UpdateOneAsync(
            b => b.title == title, 
            Builders<Book>.Update.Inc("viewCount", 1));
}