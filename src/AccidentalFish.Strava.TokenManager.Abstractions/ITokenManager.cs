using System.Threading.Tasks;

namespace AccidentalFish.Strava.TokenManager.Abstractions
{
    /// <summary>
    /// Primary interface for managing Strava access tokens
    /// </summary>
    public interface ITokenManager
    {
        /// <summary>
        /// Retrieves a token set for a given athlete and will use the stored refresh token to
        /// attempt a renew if required.
        /// </summary>
        /// <param name="athleteId">The athlete ID</param>
        /// <param name="attemptRenew">Attempt to renew the access token if required</param>
        /// <returns>A token set</returns>
        Task<TokenSet> GetTokenSetForAthleteId(string athleteId, bool attemptRenew);

        /// <summary>
        /// Retrieves a token set for a given access token and will use the stored refresh token to
        /// attempt a renew if required.
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <param name="attemptRenew">Attempt to renew the access token if required</param>
        /// <returns>A token set</returns>
        Task<TokenSet> GetTokenSetForAccessToken(string accessToken, bool attemptRenew);

        /// <summary>
        /// Translates the code returned from the Strava authentication flow into an access
        /// and refresh token and persists them in the store and cache
        /// </summary>
        /// <param name="code">The code returned from the authentication flow</param>
        /// <returns>A token set</returns>
        Task<TokenSet> TokenExchange(string code);
    }
}