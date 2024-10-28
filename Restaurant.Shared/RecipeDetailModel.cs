using System.Text.Json.Serialization;

namespace Restaurant.Shared
{
    public class RecipeDetailModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Instructions { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new();
        public List<string> Measures { get; set; } = new();
        public string YoutubeLink { get; set; } = string.Empty;
    }
}
