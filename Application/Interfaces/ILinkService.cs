using Shortly.Domain.Entities;

namespace Shortly.Application.Interfaces;

/// <summary>
/// Define the contracts for the Link related operations
/// </summary>
public interface ILinkService
{
    /// <summary>
    /// Create a Link.
    /// </summary>
    Task<Link> CreateLink(string url, long userId);

    /// <summary>
    /// Increments the clicks count for a Link.
    /// </summary>
    Task<Link> IncrementsClicks(long linkId);

    /// <summary>
    /// Retrieve a Link from this shortUrl.
    /// </summary>
    Task<Link> GetLink(string shortUrl);

    /// <summary>
    /// Retrieve all the Links.
    /// </summary>
    Task<List<Link>> GetAllLinks();

    /// <summary>
    /// Retrieve all the Links for a specific user.
    /// </summary>
    Task<List<Link>> GetLinksByUserId(long userId);
}