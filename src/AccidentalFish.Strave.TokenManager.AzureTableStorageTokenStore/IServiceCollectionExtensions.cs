using System;
using AccidentalFish.Strava.TokenManager.Abstractions;
using AccidentalFish.Strave.TokenManager.AzureTableStorageTokenStore.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace AccidentalFish.Strave.TokenManager.AzureTableStorageTokenStore
{
    // ReSharper disable once InconsistentNaming
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Register the Azure Table Storage token repository with a service collection
        /// </summary>
        /// <param name="serviceCollection">The service collection</param>
        /// <param name="options">Configuration options</param>
        /// <returns>The service collection</returns>
        public static IServiceCollection UseAzureTableStorageTokenRepository(this IServiceCollection serviceCollection, Options options)
        {
            serviceCollection
                .AddTransient<ITokenRepository, TokenRepository>()
                .AddSingleton(options)
                ;

            return serviceCollection;
        }
    }
}
