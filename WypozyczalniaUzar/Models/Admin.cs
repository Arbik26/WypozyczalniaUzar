using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

public class Admin
{
    [BsonId]
    public ObjectId Id { get; set; }

    [BsonElement("username")]
    [Required(ErrorMessage = "Username jest wymagany")]
    public string Username { get; set; } = null!;

    [BsonElement("passwordHash")]
    [Required]
    public string PasswordHash { get; set; } = null!;

    [BsonElement("email")]
    [EmailAddress(ErrorMessage = "Wpisz prawidłowy email")]
    [Required]
    public string Email { get; set; } = null!;

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}
