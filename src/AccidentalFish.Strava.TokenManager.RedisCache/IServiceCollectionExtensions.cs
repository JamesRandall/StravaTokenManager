using System;
using System.Linq;
using AccidentalFish.Strava.TokenManager.Abstractions;
using AccidentalFish.Strava.TokenManager.RedisCache.Implementation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AccidentalFish.Strava.TokenManager.RedisCache
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        public static IServiceCollection UseRedisTokenCache(this IServiceCollection serviceCollection, Options options)
        {
            serviceCollection.AddSingleton(options);
            serviceCollection.AddSingleton<IConnectionMultiplexerProvider, ConnectionMultiplexerProvider>();

            if (serviceCollection.Any(x => x.ServiceType == typeof(ITokenCache)))
            {
                serviceCollection.Replace(
                    new ServiceDescriptor(
                        typeof(ITokenCache),
                        typeof(TokenCache),
                        ServiceLifetime.Transient));
            }
            else
            {
                serviceCollection.AddTransient<ITokenCache, TokenCache>();
            }

            return serviceCollection;
        }
    }
}
