using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Shortly.Domain.Entities;

/// <summary>
/// Represents a user in the system.
/// </summary>
[Table("users")]
public class User
{
    /// <summary>
    /// Gets the unique identifier for the user.
    /// </summary>
    [Key]
    public long Id { get; private set; }

    /// <summary>
    /// Gets the email address of the user.
    /// This is a required field and must be unique.
    /// </summary>
    [Required]
    [MaxLength(320)]
    public string Email { get; private set; } = null!;

    /// <summary>
    /// Gets the password of the user.
    /// This is a required field and should be stored securely (e.g., hashed).
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Gets the collection of links associated with the user.
    /// This represents a one-to-many relationship where a user can have multiple links.
    /// </summary>
    public ICollection<Link> Links { get; private set; } = new List<Link>();

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class.
    /// This constructor is required by Entity Framework Core for materialization of the entity from the database.
    /// It should not be used directly in application code.
    /// </summary>
    private User()
    {
        // Required by EF Core.
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="User"/> class with the specified email and password.
    /// </summary>
    public User(string email, string password)
    {
        Email = string.IsNullOrWhiteSpace(email)
            ? throw new ArgumentException("Email is required", nameof(email))
            : email.Trim();

        Password = string.IsNullOrWhiteSpace(password)
            ? throw new ArgumentException("Password is required", nameof(password))
            : password;
    }
}