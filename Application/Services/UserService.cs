using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Application.Services;

/// <summary>
/// Provides services for managing users, including registration and retrieval of user information.
/// </summary>
public sealed class UserService : IUserService
{
    /// <summary>
    /// The logger instance used for logging information, warnings, and errors related to user operations.
    /// </summary>
    private readonly ILogger<UserService> _logger;

    /// <summary>
    /// The database context used for accessing the user data in the database.
    /// It allows performing CRUD operations on the User entities.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserService"/> class with the specified database context and logger.
    /// </summary>
    public UserService(AppDbContext context, ILogger<UserService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Registers a new user in the system.
    /// This method checks if the provided user object is valid, ensures that the email is not already registered,
    /// </summary>
    public async Task<User> Register(User? user)
    {
        // Validate the input user object
        if (user == null)
        {
            _logger.LogError("Registration failed: User object is null.");
            throw new ArgumentNullException(nameof(user));
        }

        _logger.LogDebug("Attempting to register user with email: {Email}", user.Email);

        // Check if the email is already registered
        var existUser = await _context.Users.AsNoTracking().AnyAsync(u => u.Email == user.Email);
        if (existUser)
        {
            _logger.LogError("Registration failed: Email {Email} is already in use.", user.Email);
            throw new InvalidOperationException("Email is already registered.");
        }

        // Hash the password before saving
        user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

        // Add the new user to the database and save changes
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User registered successfully with email: {Email} and id: {Id}.", user.Email, user.Id);
        return user;
    }

    // Get all the users
    public async Task<List<User>> GetAllUsers()
    {
        _logger.LogDebug("Retrieving all users from the database ..");
        var users = await _context.Users.AsNoTracking().ToListAsync();

        _logger.LogInformation("Retrieved {Count} users from the database.", users.Count);
        return users;
    }

    /// <inheritdoc />
    public async Task<User> Login(string email, string password)
    {
        _logger.LogDebug("Attempting login from email: {Email}", email);
        // retrieve the User from db
        var user = await _context.Users // DbSet<User>
            .AsNoTracking() // IQueryable<User>
            .FirstOrDefaultAsync(u /*:User*/ => u.Email == email); // Task<User?>

        // the user is null!
        if (user is null)
        {
            _logger.LogWarning("Login failed: No User found with email {Email}", email);
            // TODO: Change the InvalidOperationException to one part of the Domain.
            throw new InvalidOperationException($"No User found with email '{email}'.");
        }

        // the user is not null!
        if (!BCrypt.Net.BCrypt.Verify(text: password, hash: user.Password))
        {
            _logger.LogWarning("Login failed: Invalid password for User with email {Email}", email);
            throw new UnauthorizedAccessException("Invalid password");
        }

        // all ok
        // TODO: (WARNING) The user has his password and we don't want that!
        _logger.LogDebug("User logged in successfully with email: {Email}", user.Email);
        return user;
    }

    /// <inheritdoc />
    public async Task<User> GetUserByEmail(string email)
    {
        _logger.LogDebug("Retrieve User from email: {Email}", email);
        // retrieve the User from db
        var user = await _context.Users // DbSet<User>
            .AsNoTracking() // IQueryable<User>
            .FirstOrDefaultAsync(u /*:User*/ => u.Email == email); // Task<User?>

        // the user is null!
        if (user is null)
        {
            _logger.LogWarning("User not found with email {Email}", email);
            throw new InvalidOperationException($"No User found with email '{email}'.");
        }

        // all ok.
        _logger.LogDebug("User retrieved successfully by email: {Email}", user.Email);
        return user;
    }

}