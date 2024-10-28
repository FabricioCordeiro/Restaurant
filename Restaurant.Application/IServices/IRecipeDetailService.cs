using Restaurant.Shared;

namespace Restaurant.Application.IServices
{
    public interface IRecipeDetailService
    {
        event Action<RecipeDetailModel>? OnResponseReceived;
        void Publish(int idRecipe);
    }
}