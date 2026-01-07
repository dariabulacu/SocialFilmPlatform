using SocialFilmPlatform.Models;

namespace SocialFilmPlatform.Services
{
    public interface IAiService
    {
        Task<(List<string> Tags, List<string> Categories)> SuggestTagsAndCategoriesAsync(string movieTitle, string description);
        Task<(List<string> Tags, List<string> Categories)> SuggestTagsForListAsync(string listName, string listDescription, List<string> movieTitles);
    }
}
