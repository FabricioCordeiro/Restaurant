using Restaurant.Shared;
using System.Text.Json.Nodes;

namespace Restaurant.Recipes
{
#pragma warning disable CS8602
#pragma warning disable CS8601

    public class MapToModel
    {
        // Usar o mapeamento diretamente é menos verboso que usar
        // deserialização de json NESTE CONTEXTO APENAS.
        public static List<RecipeModel> MapToRecipesModel(JsonArray jsonArrayRecipes)
        {
            List<RecipeModel> recipes = new();
            foreach (var item in jsonArrayRecipes)
            {

                recipes.Add(new RecipeModel
                {
                    Id = int.TryParse(item["idMeal"]?.ToString(), out int idMeal) ? idMeal : 0,
                    Name = item["strMeal"]?.ToString() ?? string.Empty,
                    ThumbnailUrl = item["strMealThumb"]?.ToString() ?? string.Empty,
                });
            }

            return recipes;
        }

        public static RecipeDetailModel MapToRecipeDetailModel(JsonArray jsonArrayRecipe)
        {
            var jsonRecipe = jsonArrayRecipe.SingleOrDefault() ??
                throw new Exception("MapToRecipeDetailModel recebeu um jsonArray inválido.");

            var recipeDetail = new RecipeDetailModel
            {
                Id = int.TryParse(jsonRecipe["idMeal"]?.ToString(), out int idMeal) ? idMeal : 0,
                Name = jsonRecipe["strMeal"]?.ToString() ?? string.Empty,
                Category = jsonRecipe["strCategory"]?.ToString() ?? string.Empty,
                Area = jsonRecipe["strArea"]?.ToString() ?? string.Empty,
                Instructions = jsonRecipe["strInstructions"]?.ToString() ?? string.Empty,
                ThumbnailUrl = jsonRecipe["strMealThumb"]?.ToString() ?? string.Empty,
                YoutubeLink = jsonRecipe["strYoutube"]?.ToString() ?? string.Empty,
            };

            for (int i = 1; i <= 20; i++)
            {
                var ingredient = jsonRecipe[$"strIngredient{i}"]?.ToString() ?? string.Empty;
                var measure = jsonRecipe[$"strMeasure{i}"]?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(ingredient))
                    recipeDetail.Ingredients.Add(ingredient);

                if (!string.IsNullOrEmpty(measure))
                    recipeDetail.Measures.Add(measure);
            }

            return recipeDetail;
        }
    }
}