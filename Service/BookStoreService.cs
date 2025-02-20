using ExadelBookstoreAPI.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ExadelBookstoreAPI.Service;

public class BookStoreService
{
    private readonly IMongoCollection<Book> _bookCollection;

    public BookStoreService(IOptions<BookStoreDatabaseSettings> databaseSettings)
    {
        var client = new MongoClient(databaseSettings.Value.ConnectionString);
        var database = client.GetDatabase(databaseSettings.Value.DatabaseName);
        _bookCollection = database.GetCollection<Book>(databaseSettings.Value.BooksCollectionName);
    }

    public async Task<List<Book>> GetAllAsync()
        => await _bookCollection.Find(_ => true).ToListAsync();

    public async Task<Book?> GetByTitleAsync(string title)
        => await _bookCollection.Find(b => b.title == title).FirstOrDefaultAsync();

    public async Task CreateBookAsync(Book book)
        => await _bookCollection.InsertOneAsync(book);

    public async Task UpdateByTitleAsync(string title, Book book)
        => await _bookCollection.ReplaceOneAsync(b => b.title == title, book);

    public async Task DeleteByTitleAsync(string title)
        => await _bookCollection.DeleteOneAsync(b => b.title == title);
}