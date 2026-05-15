using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Shortly.Application.Interfaces;
using Shortly.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Shortly.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILinkService _linkService;
        private readonly ILogger<IndexModel> _logger;

        public IndexModel(ILinkService linkService, ILogger<IndexModel> logger)
        {
            _linkService = linkService;
            _logger = logger;
        }

        public List<Link> Links { get; set; } = new();

        [BindProperty]
        public InputModel Input { get; set; } = new();

        public class InputModel
        {
            [Required(ErrorMessage = "Please enter a URL to shorten.")]
            [Url(ErrorMessage = "Please enter a valid URL (e.g., https://example.com).")]
            public string Url { get; set; } = string.Empty;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (long.TryParse(userIdString, out var userId))
                {
                    Links = await _linkService.GetLinksByUserId(userId);
                }
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return RedirectToPage("/Login");
            }

            if (!ModelState.IsValid)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (long.TryParse(userIdString, out var userId))
                {
                    Links = await _linkService.GetLinksByUserId(userId);
                }
                return Page();
            }

            try
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (long.TryParse(userIdString, out var userId))
                {
                    await _linkService.CreateLink(Input.Url, userId);
                    _logger.LogInformation("Link created successfully for user {UserId}", userId);
                }

                // PRG pattern: redirect after POST to prevent duplicate submissions on F5
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating link.");
                ModelState.AddModelError(string.Empty, "An error occurred while creating the link.");
            }

            // Only reached on error — reload links and show the page with the error
            var userIdStringReload = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (long.TryParse(userIdStringReload, out var uId))
            {
                Links = await _linkService.GetLinksByUserId(uId);
            }

            return Page();
        }
    }
}
