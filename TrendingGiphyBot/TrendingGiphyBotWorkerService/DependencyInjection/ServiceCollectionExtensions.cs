namespace TrendingGiphyBotWorkerService.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSingletons(this IServiceCollection services, params object[] instances)
    {
        foreach (var instance in instances)
            services.AddSingleton(instance);

        return services;
    }
}
