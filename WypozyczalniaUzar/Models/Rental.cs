using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Rental
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("clientId")]
    public ObjectId ClientId { get; set; }

    [BsonElement("movieId")]
    public ObjectId MovieId { get; set; }

    [BsonElement("rentedAt")]
    public DateTime RentedAt { get; set; }

    [BsonElement("plannedReturn")]
    public DateTime PlannedReturn { get; set; }

    [BsonElement("actualReturn")]
    public DateTime? ActualReturn { get; set; }
}
