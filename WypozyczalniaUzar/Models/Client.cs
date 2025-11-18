using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Client
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; }

    [BsonElement("lastName")]
    public string LastName { get; set; }

    [BsonElement("address")]
    public string Address { get; set; }

    [BsonElement("phone")]
    public string Phone { get; set; }

    [BsonElement("registeredAt")]
    public DateTime RegisteredAt { get; set; }

    [BsonElement("login")]
    public string Login { get; set; }

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; }

    [BsonElement("isAdmin")]
    public bool IsAdmin { get; set; } = false;
}
