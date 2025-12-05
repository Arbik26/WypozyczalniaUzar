using MongoDB.Driver;
using MongoDB.Bson;
using Microsoft.Extensions.Options;

public class MongoDbService
{
    private readonly IMongoDatabase _database;
    public IMongoCollection<Movie> MoviesCollection { get; }
    public IMongoCollection<Client> ClientsCollection { get; }
    public IMongoCollection<Rental> RentalsCollection { get; }
    private bool _initialized = false;

    public MongoDbService(IOptions<MongoDbSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        _database = client.GetDatabase(settings.Value.DatabaseName);

        MoviesCollection = _database.GetCollection<Movie>("movies");
        ClientsCollection = _database.GetCollection<Client>("clients");
        RentalsCollection = _database.GetCollection<Rental>("rentals");

        // Inicjalizuj bazę asynchronicznie w tle
        _ = Task.Run(async () => await InitializeDatabaseAsync());
    }

    private async Task InitializeDatabaseAsync()
{
    if (_initialized) return;
    
    try
    {
        Console.WriteLine("🔄 Inicjalizowanie bazy danych...");
        
        // ✅ DODAJ TO - bez czekania na Clients (mogą być puste)
        _initialized = true;
        Console.WriteLine("✅ Baza danych GOTOWA (bez czekania na kolekcje)!");
        
        // Sprawdzić czy baza ma dane (ale nie blokuj inicjalizacji)
        try
        {
            var movieCount = await MoviesCollection.CountDocumentsAsync(_ => true);
            Console.WriteLine($"📊 Liczba filmów w bazie: {movieCount}");
            
            if (movieCount == 0)
            {
                Console.WriteLine("➕ Dodawanie testowych filmów...");
                
                var testMovies = new List<Movie>
                {
                    new Movie
                    {
                        Title = "Interstellar",
                        Director = "Christopher Nolan",
                        Genre = "Sci-Fi",
                        Length = 169,
                        Rating = 8.6,
                        Description = "A team of explorers travel through a wormhole in space",
                        Actors = new List<string> { "Matthew McConaughey", "Anne Hathaway" },
                        AddedAt = DateTime.Now,
                        Available = true
                    }
                };

                await MoviesCollection.InsertManyAsync(testMovies);
                Console.WriteLine("✅ Testowe filmy dodane do bazy!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"⚠️  Problem z filmami: {ex.Message}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Błąd inicjalizacji bazy: {ex.Message}");
        _initialized = true; // ← WAŻNE: ustaw na true i tak!
    }
}


    // ========== MOVIES ==========
    public async Task<List<Movie>> GetAllMoviesAsync()
    {
        await EnsureInitialized();
        return await MoviesCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Movie> GetMovieByIdAsync(string id)
    {
        await EnsureInitialized();
        return await MoviesCollection.Find(m => m.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
    }

    public async Task<List<Movie>> GetAvailableMoviesAsync()
    {
        await EnsureInitialized();
        Console.WriteLine("🎬 Pobieranie dostępnych filmów...");
        var result = await MoviesCollection.Find(m => m.Available).ToListAsync();
        Console.WriteLine($"📽️ Znaleziono {result.Count} dostępnych filmów");
        return result;
    }

    public async Task<List<Movie>> SearchMoviesByTitleAsync(string title)
    {
        await EnsureInitialized();
        return await MoviesCollection.Find(m => m.Title.Contains(title)).ToListAsync();
    }

    public async Task<List<Movie>> SearchMoviesByGenreAsync(string genre)
    {
        await EnsureInitialized();
        return await MoviesCollection.Find(m => m.Genre.Contains(genre)).ToListAsync();
    }

    public async Task AddMovieAsync(Movie movie)
    {
        await EnsureInitialized();
        await MoviesCollection.InsertOneAsync(movie);
    }

    public async Task UpdateMovieAsync(string id, Movie movie)
    {
        await EnsureInitialized();
        await MoviesCollection.ReplaceOneAsync(m => m.Id == ObjectId.Parse(id), movie);
    }

    public async Task DeleteMovieAsync(string id)
    {
        await EnsureInitialized();
        await MoviesCollection.DeleteOneAsync(m => m.Id == ObjectId.Parse(id));
    }

    // ========== CLIENTS - AUTHENTICATION ==========
    
    /// <summary>
    /// Wyszukaj klienta po loginie (dla logowania)
    /// </summary>
    public async Task<Client?> GetClientByLoginAsync(string login)
    {
        await EnsureInitialized();
        return await ClientsCollection.Find(c => c.Login == login).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Wyszukaj klienta po emailu (sprawdzenie duplikatu przy rejestracji)
    /// </summary>
    public async Task<Client?> GetClientByEmailAsync(string email)
    {
        await EnsureInitialized();
        return await ClientsCollection.Find(c => c.Email == email).FirstOrDefaultAsync();
    }

    /// <summary>
    /// Stwórz nowego klienta (rejestracja)
    /// </summary>
    public async Task CreateClientAsync(Client client)
    {
        await EnsureInitialized();
        await ClientsCollection.InsertOneAsync(client);
    }

    // ========== CLIENTS - STANDARD CRUD ==========
    
    public async Task<List<Client>> GetAllClientsAsync()
    {
        await EnsureInitialized();
        return await ClientsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<Client> GetClientByIdAsync(string id)
    {
        await EnsureInitialized();
        return await ClientsCollection.Find(c => c.Id == ObjectId.Parse(id)).FirstOrDefaultAsync();
    }

    public async Task AddClientAsync(Client client)
    {
        await EnsureInitialized();
        await ClientsCollection.InsertOneAsync(client);
    }

    public async Task UpdateClientAsync(string id, Client client)
    {
        await EnsureInitialized();
        await ClientsCollection.ReplaceOneAsync(c => c.Id == ObjectId.Parse(id), client);
    }

    public async Task DeleteClientAsync(string id)
    {
        await EnsureInitialized();
        await ClientsCollection.DeleteOneAsync(c => c.Id == ObjectId.Parse(id));
    }

    // ========== RENTALS ==========
    public async Task<List<Rental>> GetAllRentalsAsync()
    {
        await EnsureInitialized();
        return await RentalsCollection.Find(_ => true).ToListAsync();
    }

    public async Task<List<Rental>> GetRentalsByClientAsync(string clientId)
    {
        await EnsureInitialized();
        return await RentalsCollection.Find(r => r.ClientId == ObjectId.Parse(clientId)).ToListAsync();
    }

    public async Task<List<Rental>> GetActiveRentalsByClientAsync(string clientId)
    {
        await EnsureInitialized();
        return await RentalsCollection.Find(r => r.ClientId == ObjectId.Parse(clientId) && r.ActualReturn == null).ToListAsync();
    }

    public async Task AddRentalAsync(Rental rental)
    {
        await EnsureInitialized();
        await RentalsCollection.InsertOneAsync(rental);
    }

    public async Task UpdateRentalAsync(string id, Rental rental)
    {
        await EnsureInitialized();
        await RentalsCollection.ReplaceOneAsync(r => r.Id == ObjectId.Parse(id), rental);
    }

    public async Task<int> GetActiveRentalCountAsync(string clientId)
    {
        await EnsureInitialized();
        var count = await RentalsCollection.CountDocumentsAsync(
            r => r.ClientId == ObjectId.Parse(clientId) && r.ActualReturn == null
        );
        return (int)count;
    }

    // ========== HELPER METHODS ==========
    
   private async Task EnsureInitialized()
{
    int timeout = 0;
    int maxTimeout = 50; // Zwiększone z 30 na 50 (5 sekund zamiast 3)
    
    while (!_initialized && timeout < maxTimeout)
    {
        await Task.Delay(100);
        timeout++;
    }
    
    if (!_initialized)
    {
        Console.WriteLine($"⚠️  UWAGA: Baza danych nie całkowicie zainicjalizowana, ale kontynuuję...");
        // Nie rzucaj wyjątku - pozwól kontynuować!
    }
}


    // ========== ADMINS ==========
    public IMongoCollection<Admin> GetAdminsCollection()
        => _database.GetCollection<Admin>("admins");

    public async Task<Admin> GetAdminByUsernameAsync(string username)
        => await GetAdminsCollection().Find(a => a.Username == username).FirstOrDefaultAsync();

    public async Task<string> CreateAdminAsync(Admin admin)
    {
        await GetAdminsCollection().InsertOneAsync(admin);
        return admin.Id.ToString();
    }
}
