using Restaurant.Recipes;
using Restaurant.Recipes.IServices;
using Restaurant.Recipes.Services;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        services.AddSingleton<IRabbitMQService, RabbitMQService>();
        services.AddHttpClient<IRecipeService, RecipeService>();
        services.AddHostedService<Worker>();
    })
    .Build();

await host.RunAsync();