using System.Threading.Tasks;

namespace AccidentalFish.Strava.TokenManager.Abstractions
{
    /// <summary>
    /// Interface for long term token persistence
    /// </summary>
    public interface ITokenRepository : ITokenStoreCore
    {
        /// <summary>
        /// Retrieves a token set for an athlete
        /// </summary>
        /// <param name="athleteId">The athlete ID</param>
        /// <returns>A token set including refresh token</returns>
        Task<RepositoryEnrichedTokenSet> GetForAthlete(string athleteId);
    }
}
