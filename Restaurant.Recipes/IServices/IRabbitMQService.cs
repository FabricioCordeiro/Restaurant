using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Restaurant.Recipes.IServices
{
    public interface IRabbitMQService
    {
        void StartListening();
        void PublishRecipesResponse(string response);
        void PublishRecipeDetailResponse(string response);
        event Action<string>? OnMessageRecipesReceived;
        event Action<string>? OnMessageRecipeDetailReceived;
    }
}
