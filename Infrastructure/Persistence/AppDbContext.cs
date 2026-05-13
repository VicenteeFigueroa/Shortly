using Microsoft.EntityFrameworkCore;
using Shortly.Domain.Entities;

namespace Shortly.Infrastructure.Persistence;

/// <summary>
/// Represents the application's database context, which is responsible for managing the connection to the database and
/// providing access to the entities (User and Link) through DbSet properties.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class with the specified options.
    /// </summary>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        // The base constructor is called to initialize the DbContext with the provided options.
    }

    /// <summary>
    /// Gets or sets the DbSet of User entities, which represents the collection of users in the database.
    /// </summary>
    public DbSet<User> Users { get; set; } = null!;

    /// <summary>
    /// Gets or sets the DbSet of Link entities, which represents the collection of links in the database.
    /// </summary>
    public DbSet<Link> Links { get; set; } = null!;
}
