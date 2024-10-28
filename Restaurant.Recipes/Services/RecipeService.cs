using Restaurant.Recipes.IServices;
using Restaurant.Shared;
using System.Text.Json.Nodes;
namespace Restaurant.Recipes.Services;

public class RecipeService : IRecipeService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<RecipeService> _logger;
    private readonly string _baseUrl;
    private readonly string _apiKey;

    // Repare que este serviço é declarado como AddHttpClient. Isso é importante pq faço a injeção do HttpClient
    // AddHttpClient ao inves de um singleton por exemplo, trata de alguns problemas que podem ocorrer com o uso do HttpClient
    public RecipeService(HttpClient httpClient, IConfiguration configuration, ILogger<RecipeService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
        _baseUrl = configuration["ApiConfig:BaseUrl"];
        _apiKey = configuration["ApiConfig:ApiKey"];
    }

    // Obtém os detalhes de uma determinada receita.
    public async Task<RecipeDetailModel?> GetRecipeDetailAsync(int idRecipe)
    {
        try
        {
            string url = $"{_baseUrl}/{_apiKey}/lookup.php?i={idRecipe}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogTrace($"{url} retornou OK");

                var jsonContent = await response.Content.ReadAsStringAsync();
                var jsonNode = JsonNode.Parse(jsonContent);

                return jsonNode?["meals"] is JsonArray recipeJson ? MapToModel.MapToRecipeDetailModel(recipeJson) : null;
            }
            else
            {
                _logger.LogError("Falha ao recuperar dados. Código de status: {StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Ocorreu um erro ao buscar os dados da receita: {Message}", ex.Message);
            return null;
        }
    }

    // Obtém uma lista de receitas pelo nome.
    public async Task<List<RecipeModel>?> GetRecipesAsync(string recipeName)
    {
        try
        {
            string url = $"{_baseUrl}/{_apiKey}/search.php?s={recipeName}";
            var response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogTrace($"{url} retornou OK");

                var jsonContent = await response.Content.ReadAsStringAsync();
                var jsonNode = JsonNode.Parse(jsonContent);

                return jsonNode?["meals"] is JsonArray recipesJson ? MapToModel.MapToRecipesModel(recipesJson) : null;
            }
            else
            {
                _logger.LogError($"Falha ao recuperar dados de {url}. Código de status: {response.StatusCode}", response.StatusCode);
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Ocorreu um erro ao buscar receitas: {ex.Message}");
            return null;
        }
    }
}
