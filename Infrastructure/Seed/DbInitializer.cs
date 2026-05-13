using Microsoft.EntityFrameworkCore;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Infrastructure.Seed;

/// <summary>
/// Provides methods for initializing the database, including applying pending migrations
/// and seeding default data for development purposes.
/// </summary>
public static class DbInitializer
{
    /// <summary>
    /// Initializes the database by applying pending migrations and seeding default data.
    /// This method should be called during application startup to ensure the database is ready.
    /// </summary>
    public static async Task InitializeAsync(AppDbContext context, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(logger);

        logger.LogDebug("Applying pending migrations ..");
        await context.Database.MigrateAsync();
        logger.LogInformation("Migrations applied successfully.");

        // Seed default data
        await SeedUsersAsync(context, logger);
        await SeedLinksAsync(context, logger);
    }

    /// <summary>
    /// Seeds the database with default users if no users exist.
    /// Passwords are hashed using BCrypt before being stored.
    /// </summary>
    private static async Task SeedUsersAsync(AppDbContext context, ILogger logger)
    {
        // Skip seeding if users already exist
        if (await context.Users.AnyAsync())
        {
            logger.LogDebug("Users table already has data, skipping user seed.");
            return;
        }

        logger.LogDebug("Seeding default users ..");

        var users = new List<User>
        {
            new("admin@shortly.com", BCrypt.Net.BCrypt.HashPassword("Admin123!")),
            new("user@shortly.com", BCrypt.Net.BCrypt.HashPassword("User123!"))
        };

        context.Users.AddRange(users);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} default users successfully.", users.Count);
    }

    /// <summary>
    /// Seeds the database with default links if no links exist.
    /// Links are associated with the first seeded user.
    /// </summary>
    private static async Task SeedLinksAsync(AppDbContext context, ILogger logger)
    {
        // Skip seeding if links already exist
        if (await context.Links.AnyAsync())
        {
            logger.LogDebug("Links table already has data, skipping link seed.");
            return;
        }

        // Get the first user to associate the links with
        var user = await context.Users.FirstOrDefaultAsync();
        if (user is null)
        {
            logger.LogWarning("No users found to associate links with, skipping link seed.");
            return;
        }

        logger.LogDebug("Seeding default links for user Id: {UserId} ..", user.Id);

        var links = new List<Link>
        {
            new("https://github.com", "gh001", user.Id),
            new("https://google.com", "gg002", user.Id),
            new("https://stackoverflow.com", "so003", user.Id)
        };

        context.Links.AddRange(links);
        await context.SaveChangesAsync();

        logger.LogInformation("Seeded {Count} default links successfully.", links.Count);
    }
}
