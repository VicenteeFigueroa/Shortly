using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shortly.Domain.Entities;

/// <summary>
/// Represents a shortened link in the system.
/// </summary>
[Table("links")]
[Index(nameof(ShortUrl), IsUnique = true)]
public class Link
{
    /// <summary>
    /// Gets the unique identifier for the link.
    /// </summary>
    [Key]
    public long Id { get; private set; }

    /// <summary>
    /// Gets the original URL that the link points to.
    /// </summary>
    [Required]
    [MaxLength(20248)]
    public string Url { get; private set; } = null!;

    /// <summary>
    /// Gets the shortened URL that can be used to access the original URL.
    /// </summary>
    [Required]
    [MaxLength(32)]
    public string ShortUrl { get; private set; } = null!;

    /// <summary>
    /// Gets the number of times the shortened URL has been accessed.
    /// </summary>
    [Required]
    public int Clicks { get; private set; }

    /// <summary>
    /// Gets the foreign key to the user who created the link.
    /// </summary>
    [ForeignKey(nameof(User))]
    public long UserId { get; private set; }

    /// <summary>
    /// Gets the user who created the link.
    /// </summary>
    public User User { get; private set; } = null!;

    /// <summary>
    /// Initializes a new instance of the <see cref="Link"/> class.
    /// </summary>
    private Link()
    {
        // Required by EF Core
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Link"/> class with the specified URL, short URL, and user ID.
    /// </summary>
    public Link(string url, string shortUrl, long userId)
    {
        Url = string.IsNullOrWhiteSpace(url)
            ? throw new ArgumentException("URL is required.", nameof(url))
            : url.Trim();

        ShortUrl = string.IsNullOrWhiteSpace(shortUrl)
            ? throw new ArgumentException("ShortUrl is required.", nameof(shortUrl))
            : shortUrl.Trim();

        UserId = userId > 0
            ? userId
            : throw new ArgumentOutOfRangeException(nameof(userId), "UserId must be greater than zero.");

        Clicks = 0;
    }

    /// <summary>
    /// Increments the click count for the link by one.
    /// </summary>
    public void IncrementClicks() => Clicks++;
}