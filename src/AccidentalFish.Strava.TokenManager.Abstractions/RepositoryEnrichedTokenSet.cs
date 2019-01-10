namespace AccidentalFish.Strava.TokenManager.Abstractions
{
    /// <summary>
    /// Used by token storage to attach a refresh token to the tokens exposed by the token
    /// manager (the token manager does not expose the refresh token - it is considered more
    /// sensitive than the access token due to its longer life and hiding it helps prevent
    /// leakage errors)
    /// </summary>
    public class RepositoryEnrichedTokenSet : TokenSet
    {
        /// <summary>
        /// The refresh token
        /// </summary>
        public string RefreshToken { get; set; }
    }
}