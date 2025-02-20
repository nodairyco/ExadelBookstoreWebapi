using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Swashbuckle.AspNetCore.Annotations;

namespace ExadelBookstoreAPI.Models;

public class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    [SwaggerIgnore]
    public string? Id { get; set; } 
    public string title { get; set; }
    public int publicationYear { get; set; }
    public string authorName { get; set; }
    public int viewCount { get; set; }

    public double GetPopularity()
        => 0.5 * this.viewCount + (DateTime.Today.Year - this.publicationYear);
}