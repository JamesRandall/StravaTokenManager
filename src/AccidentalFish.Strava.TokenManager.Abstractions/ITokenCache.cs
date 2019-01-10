namespace AccidentalFish.Strava.TokenManager.Abstractions
{
    /// <summary>
    /// Interface for short term fast access token storage. Primarily used to support the
    /// get by access token operation required in API auth scenarios
    /// </summary>
    public interface ITokenCache : ITokenStoreCore
    {
        
    }
}
