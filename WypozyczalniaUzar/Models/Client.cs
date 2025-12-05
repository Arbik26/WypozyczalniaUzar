using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

public class Client
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("firstName")]
    public string FirstName { get; set; } = string.Empty;

    [BsonElement("lastName")]
    public string LastName { get; set; } = string.Empty;

    [BsonElement("address")]
    public string Address { get; set; } = string.Empty;

    [BsonElement("phone")]
    public string Phone { get; set; } = string.Empty;

    [BsonElement("login")]
    public string Login { get; set; } = string.Empty;

    [BsonElement("passwordHash")]
    public string PasswordHash { get; set; } = string.Empty;

    [BsonElement("email")]
    public string Email { get; set; } = string.Empty;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    [BsonElement("updatedAt")]
    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
