using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace ExadelBookstoreAPI.Models;

public class Book
{
    /// <summary>
    /// Id given by MongoDB also the primary key. Ignored by JSON and Swagger to omit any issues
    /// </summary>
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerIgnore]
    [JsonIgnore]
    public string? Id { get; set; } 
    /// <summary>
    /// Title of the book
    /// </summary>
    public string title { get; set; }
    /// <summary>
    /// Publication year of the book
    /// </summary>
    public int publicationYear { get; set; }
    /// <summary>
    /// Author Name of the book
    /// </summary>
    public string authorName { get; set; }
    /// <summary>
    /// Number of times this book was viewed. Updated every time a book is viewed.
    /// </summary>
    public int viewCount { get; set; } = 0;

    /// <summary>
    /// Here for soft deletion of the book. Ignored by JSON and Swagger to omit any issues that may arise.
    /// </summary>
    [JsonIgnore]
    [SwaggerIgnore]
    public bool isDeleted { get; set; } = false;

    /// <summary>
    /// Returns the popularity of the book with the given formula: viewCount/2 + yearsSincePublication * 2
    /// </summary>
    public double GetPopularity()
        => 0.5 * viewCount + 2*(DateTime.Today.Year - publicationYear);
}