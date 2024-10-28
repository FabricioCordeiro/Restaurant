using Restaurant.Recipes.IServices;
using System.Text.Json;

namespace Restaurant.Recipes
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IRecipeService _recipeService;
        private readonly IRabbitMQService _rabbitMQService;


        public Worker(ILogger<Worker> logger, IRecipeService recipeService, IRabbitMQService rabbitMQService)
        {
            _logger = logger;
            _recipeService = recipeService;
            _rabbitMQService = rabbitMQService;

            // Assinando eventos
            _rabbitMQService.OnMessageRecipesReceived += HandleRecipesReceived;
            _rabbitMQService.OnMessageRecipeDetailReceived += HandleRecipeDetailReceived;
        }

        // Manipulador para mensagem recebida na fila de Recipes
        private async void HandleRecipesReceived(string message)
        {
            _logger.LogTrace("Recebendo requisição em Recipes: {Message}", message);

            var recipe = await _recipeService.GetRecipesAsync(message);
            var responseMessage = JsonSerializer.Serialize(recipe);

            _rabbitMQService.PublishRecipesResponse(responseMessage);
            _logger.LogTrace("Resposta enviada para Recipes: {Response}", responseMessage);
        }

        // Manipulador para mensagem recebida na fila de RecipeDetail
        private async void HandleRecipeDetailReceived(string message)
        {
            _logger.LogTrace("Recebendo requisição em RecipeDetail: {Message}", message);

            var recipe = await _recipeService.GetRecipeDetailAsync(Convert.ToInt32(message));
            var responseMessage = JsonSerializer.Serialize(recipe);

            _rabbitMQService.PublishRecipeDetailResponse(responseMessage);
            _logger.LogTrace("Resposta enviada para RecipesDetail: {Response}", responseMessage);
        }

        // Inicia o Listening
        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _rabbitMQService.StartListening();
            _logger.LogTrace("Worker está aguardando mensagens na fila.");
            return Task.CompletedTask;
        }
    }
}
