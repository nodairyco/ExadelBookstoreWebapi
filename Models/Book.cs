using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ExadelBookstoreAPI.Models;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; } 
    public string title { get; set; }
    public int publicationYear { get; set; }
    public string authorName { get; set; }
    public int viewCount { get; set; }
}