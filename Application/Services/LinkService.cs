using Microsoft.EntityFrameworkCore;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using Shortly.Infrastructure.Persistence;

namespace Shortly.Application.Services;

/// <summary>
/// Provides services for managing links, including creation, retrieval, and click tracking.
/// </summary>
public sealed class LinkService : ILinkService
{
    /// <summary>
    /// The logger instance used for logging information, warnings, and errors related to link operations.
    /// </summary>
    private readonly ILogger<LinkService> _logger;

    /// <summary>
    /// The database context used for accessing the link data in the database.
    /// It allows performing CRUD operations on the Link entities.
    /// </summary>
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkService"/> class with the specified database context and logger.
    /// </summary>
    public LinkService(AppDbContext context, ILogger<LinkService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <inheritdoc />
    public async Task<Link> CreateLink(string url, long userId)
    {
        _logger.LogDebug("Attempting to create link for URL: {Url} by user: {UserId}", url, userId);

        // Validate the URL
        if (string.IsNullOrWhiteSpace(url))
        {
            _logger.LogError("Link creation failed: URL is null or empty.");
            throw new ArgumentException("URL is required.", nameof(url));
        }

        // Validate the userId
        if (userId <= 0)
        {
            _logger.LogError("Link creation failed: UserId {UserId} is invalid.", userId);
            throw new ArgumentOutOfRangeException(nameof(userId), "UserId must be greater than zero.");
        }

        // Verify that the user exists
        var userExists = await _context.Users.AsNoTracking().AnyAsync(u => u.Id == userId);
        if (!userExists)
        {
            _logger.LogError("Link creation failed: No user found with Id {UserId}.", userId);
            throw new InvalidOperationException($"No User found with Id '{userId}'.");
        }

        // Generate a unique short URL
        var shortUrl = Guid.NewGuid().ToString("N")[..8];

        // Create the link entity
        var link = new Link(url, shortUrl, userId);

        // Add the new link to the database and save changes
        _context.Links.Add(link);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Link created successfully with Id: {Id}, ShortUrl: {ShortUrl} for user: {UserId}.",
            link.Id, link.ShortUrl, link.UserId);
        return link;
    }

    /// <inheritdoc />
    public async Task<Link> IncrementsClicks(long linkId)
    {
        _logger.LogDebug("Attempting to increment clicks for link Id: {LinkId}", linkId);

        // Retrieve the link (tracked, since we need to update it)
        var link = await _context.Links.FirstOrDefaultAsync(l => l.Id == linkId);

        if (link is null)
        {
            _logger.LogWarning("Increment clicks failed: No link found with Id {LinkId}.", linkId);
            throw new InvalidOperationException($"No Link found with Id '{linkId}'.");
        }

        // Increment the click count
        link.IncrementClicks();
        await _context.SaveChangesAsync();

        _logger.LogInformation("Clicks incremented for link Id: {LinkId}. Total clicks: {Clicks}.",
            link.Id, link.Clicks);
        return link;
    }

    /// <inheritdoc />
    public async Task<Link> GetLink(string shortUrl)
    {
        _logger.LogDebug("Retrieving link with ShortUrl: {ShortUrl}", shortUrl);

        // Retrieve the link from db
        var link = await _context.Links // DbSet<Link>
            .AsNoTracking() // IQueryable<Link>
            .FirstOrDefaultAsync(l /*:Link*/ => l.ShortUrl == shortUrl); // Task<Link?>

        // the link is null!
        if (link is null)
        {
            _logger.LogWarning("Link not found with ShortUrl: {ShortUrl}", shortUrl);
            throw new InvalidOperationException($"No Link found with ShortUrl '{shortUrl}'.");
        }

        // all ok.
        _logger.LogDebug("Link retrieved successfully with ShortUrl: {ShortUrl}", link.ShortUrl);
        return link;
    }

    /// <inheritdoc />
    public async Task<List<Link>> GetAllLinks()
    {
        _logger.LogDebug("Retrieving all links from the database ..");
        var links = await _context.Links.AsNoTracking().ToListAsync();

        _logger.LogInformation("Retrieved {Count} links from the database.", links.Count);
        return links;
    }

    /// <inheritdoc />
    public async Task<List<Link>> GetLinksByUserId(long userId)
    {
        _logger.LogDebug("Retrieving links for user Id: {UserId}", userId);
        
        var links = await _context.Links
            .AsNoTracking()
            .Where(l => l.UserId == userId)
            .OrderByDescending(l => l.Id)
            .ToListAsync();

        _logger.LogInformation("Retrieved {Count} links for user Id: {UserId}.", links.Count, userId);
        return links;
    }
}