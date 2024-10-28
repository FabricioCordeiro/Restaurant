using Restaurant.Shared;

namespace Restaurant.Application.IServices
{
    public interface IRecipesService
    {
        event Action<List<RecipeModel>>? OnResponseReceived;
        void Publish(string recipeName);
    }
}