using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Movie
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("title")]
    public string Title { get; set; }

    [BsonElement("genre")]
    public string Genre { get; set; }

    [BsonElement("director")]
    public string Director { get; set; }

    [BsonElement("length")]
    public int Length { get; set; }

    [BsonElement("rating")]
    public double Rating { get; set; }

    [BsonElement("description")]
    public string Description { get; set; }

    [BsonElement("actors")]
    public List<string> Actors { get; set; }

    [BsonElement("addedAt")]
    public DateTime AddedAt { get; set; }

    [BsonElement("available")]
    public bool Available { get; set; } = true;
}
