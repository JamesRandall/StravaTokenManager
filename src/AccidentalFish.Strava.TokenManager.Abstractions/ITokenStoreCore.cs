using System;
using System.Threading.Tasks;

namespace AccidentalFish.Strava.TokenManager.Abstractions
{
    /// <summary>
    /// Token storage operations common to short and long term storage
    /// </summary>
    public interface ITokenStoreCore
    {
        /// <summary>
        /// Save token details
        /// </summary>
        /// <param name="athleteId">Athlete ID</param>
        /// <param name="accessToken">Access token</param>
        /// <param name="refreshToken">Refresh token</param>
        /// <param name="accessTokenExpiresAtUtc">Expiry date of the access token</param>
        /// <returns></returns>
        Task SaveTokenSet(string athleteId, string accessToken, string refreshToken,
            DateTime accessTokenExpiresAtUtc);

        /// <summary>
        /// Return a token set given an access token
        /// </summary>
        /// <param name="accessToken">The access token</param>
        /// <returns>A token set including the refresh token</returns>
        Task<RepositoryEnrichedTokenSet> GetForAccessToken(string accessToken);

        /// <summary>
        /// Delete the token from the store
        /// </summary>
        /// <param name="accessToken">The access token</param>
        Task DeleteTokenSetForAccessToken(string accessToken);
    }
}
