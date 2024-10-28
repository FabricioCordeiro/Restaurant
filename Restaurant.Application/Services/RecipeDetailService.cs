using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Restaurant.Application.IServices;
using Restaurant.Shared;

namespace Restaurant.Application.Services
{
    public class RecipeDetailService : IRecipeDetailService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly string[] _queueNames = { 
            "recipeDetailQueue", 
            "responseRecipeDetailQueue" 
        };

        public event Action<RecipeDetailModel>? OnResponseReceived;

        public RecipeDetailService(IConfiguration configuration)
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

                StartConsumer();
            }
        }

        private void StartConsumer()
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var response = JsonSerializer.Deserialize<RecipeDetailModel>(message);

                if (response != null)
                {
                    OnResponseReceived?.Invoke(response);
                }

                // Envia o ack manual para garantir que a mensagem foi processada.
                _channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
            };

            // Inicia o consumo da fila de resposta com ack manual.
            _channel.BasicConsume(queue: _queueNames[1], autoAck: false, consumer: consumer);
        }

        public void Publish(int idRecipe)
        {
            var messageBytes = Encoding.UTF8.GetBytes(idRecipe.ToString());
            _channel.BasicPublish(
                exchange: string.Empty,
                routingKey: _queueNames[0],
                basicProperties: null,
                body: messageBytes
            );
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this); //CA1816. Recomendação do Intelisense.
            _channel?.Close();
            _connection?.Close();
        }
    }
}