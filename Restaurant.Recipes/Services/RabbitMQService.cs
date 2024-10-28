using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System.Text;
using Restaurant.Recipes.IServices;

namespace Restaurant.Recipes.Services
{
    public class RabbitMQService : IRabbitMQService
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string[] _queueNames = {
            "recipesQueue",
            "responseRecipesQueue",
            "recipeDetailQueue",
            "responseRecipeDetailQueue",
        };

        public event Action<string>? OnMessageRecipesReceived;
        public event Action<string>? OnMessageRecipeDetailReceived;

        // No contexto do Worker Service, um serviço exclusivo para a declaração
        // e tratametno dos Consumers e Subscribers.
        // Ao menos, no contexto deste projeto.
        public RabbitMQService(IConfiguration configuration)
        {
            var factory = new ConnectionFactory() { Uri = new Uri(configuration["RabbitMQ:Uri"]) };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            foreach (var queueName in _queueNames)
            {
                _channel.QueueDeclare(
                    queue: queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null
                );
            }
        }

        public void StartListening()
        {
            // Escuta a fila de Recipes
            var consumerRecipes = new EventingBasicConsumer(_channel);
            consumerRecipes.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                OnMessageRecipesReceived?.Invoke(message);
            };
            _channel.BasicConsume(
                queue: _queueNames[0], 
                autoAck: true, 
                consumer: consumerRecipes
            );

            // Escuta a fila de RecipeDetail
            var consumerRecipeDetail = new EventingBasicConsumer(_channel);
            consumerRecipeDetail.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var response = Encoding.UTF8.GetString(body);
                OnMessageRecipeDetailReceived?.Invoke(response);
            };
            _channel.BasicConsume(
                queue: _queueNames[2], 
                autoAck: true, 
                consumer: consumerRecipeDetail
            );
        }

        // Publica a resposta em recipesQueue
        public void PublishRecipesResponse(string response)
        {
            var body = Encoding.UTF8.GetBytes(response);
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _queueNames[1],
                basicProperties: null,
                body: body
            );
        }

        // Publica a resposta em recipeDetailQueue
        public void PublishRecipeDetailResponse(string response)
        {
            var body = Encoding.UTF8.GetBytes(response);
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _queueNames[3],
                basicProperties: null,
                body: body
            );
        }
    }
}