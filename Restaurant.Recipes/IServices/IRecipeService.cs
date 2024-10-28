using Restaurant.Shared;

namespace Restaurant.Recipes.IServices
{
    public interface IRecipeService
    {
        Task<RecipeDetailModel?> GetRecipeDetailAsync(int idRecipe);
        Task<List<RecipeModel>?> GetRecipesAsync(string recipeName);
    }
}
