using StackExchange.Redis;

namespace AccidentalFish.Strava.TokenManager.RedisCache.Implementation
{
    internal interface IConnectionMultiplexerProvider
    {
        ConnectionMultiplexer Get();
    }
}