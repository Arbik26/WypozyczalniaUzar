using System.Security.Cryptography;
using System.Text;

public class AuthenticationService
{
    private readonly MongoDbService _mongoDbService;

    public AuthenticationService(MongoDbService mongoDbService)
    {
        _mongoDbService = mongoDbService;
    }

    /// <summary>
    /// Rejestruje nowego użytkownika
    /// </summary>
    public async Task<(bool success, string message)> RegisterAsync(
        string firstName, string lastName, string address, 
        string phone, string login, string password, string email)
    {
        try
        {
            Console.WriteLine($"📝 Rejestracja: {login}");

            // Sprawdzenie loginu
            var existingClient = await _mongoDbService.GetClientByLoginAsync(login);
            if (existingClient != null)
            {
                Console.WriteLine($"❌ Login '{login}' już istnieje");
                return (false, "Login już istnieje w systemie");
            }

            // Sprawdzenie emaila
            var existingEmail = await _mongoDbService.GetClientByEmailAsync(email);
            if (existingEmail != null)
            {
                Console.WriteLine($"❌ Email '{email}' już istnieje");
                return (false, "Email już istnieje w systemie");
            }

            // Hash hasła
            string passwordHash = HashPassword(password);
            Console.WriteLine($"🔐 Hasło zahashowane: {passwordHash.Substring(0, 20)}...");

            // Stwórz nowego klienta
            var newClient = new Client
            {
                FirstName = firstName,
                LastName = lastName,
                Address = address,
                Phone = phone,
                Login = login,
                PasswordHash = passwordHash,
                Email = email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            // Dodaj do bazy
            await _mongoDbService.CreateClientAsync(newClient);
            Console.WriteLine($"✅ Użytkownik '{login}' zarejestrowany!");

            return (true, "Rejestracja pomyślna! Możesz się teraz zalogować.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Błąd rejestracji: {ex.Message}");
            return (false, $"Błąd: {ex.Message}");
        }
    }

    /// <summary>
    /// Loguje użytkownika
    /// </summary>
    public async Task<(bool success, Client? client, string message)> LoginAsync(
        string login, string password)
    {
        try
        {
            Console.WriteLine($"🔓 Logowanie: {login}");

            // Wyszukaj użytkownika
            var client = await _mongoDbService.GetClientByLoginAsync(login);
            if (client == null)
            {
                Console.WriteLine($"❌ Użytkownik '{login}' nie znaleziony");
                return (false, null, "Login lub hasło jest nieprawidłowe");
            }

            Console.WriteLine($"✓ Znaleziono użytkownika: {login}");
            Console.WriteLine($"  Hasło w bazie: {client.PasswordHash.Substring(0, 20)}...");

            // Porównaj hasła
            string inputHash = HashPassword(password);
            Console.WriteLine($"  Wpisane hasło: {inputHash.Substring(0, 20)}...");

            if (!VerifyPassword(password, client.PasswordHash))
            {
                Console.WriteLine($"❌ Hasło się nie zgadza!");
                return (false, null, "Login lub hasło jest nieprawidłowe");
            }

            Console.WriteLine($"✅ Login '{login}' pomyślny!");
            return (true, client, "Zalogowano pomyślnie!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Błąd logowania: {ex.Message}");
            return (false, null, $"Błąd: {ex.Message}");
        }
    }

    /// <summary>
    /// Hashuje hasło
    /// </summary>
    private string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    /// <summary>
    /// Weryfikuje hasło
    /// </summary>
    private bool VerifyPassword(string password, string hash)
    {
        var hashOfInput = HashPassword(password);
        return hashOfInput == hash;
    }
}
