using System.Linq;
using AccidentalFish.Strava.TokenManager.Abstractions;
using AccidentalFish.Strava.TokenManager.Implementation;
using Microsoft.Extensions.DependencyInjection;

namespace AccidentalFish.Strava.TokenManager
{
    // ReSharper disable once InconsistentNaming
    /// <summary>
    /// Dependency registration
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the token manager with a IServiceCollection
        /// </summary>
        public static IServiceCollection UseStravaTokenManager(
            this IServiceCollection serviceCollection,
            Options options)
        {
            serviceCollection
                .AddSingleton(options)
                .AddTransient<ITokenManager, Implementation.TokenManager>()
                ;

            if (serviceCollection.All(x => x.ServiceType != typeof(ITokenCache)))
            {
                serviceCollection.AddSingleton<ITokenCache, NoCache>();
            }

            return serviceCollection;
        }
    }
}
