using StackExchange.Redis;

namespace AccidentalFish.Strava.TokenManager.RedisCache.Implementation
{
    internal class ConnectionMultiplexerProvider : IConnectionMultiplexerProvider
    {
        private readonly ConnectionMultiplexer _multiplexer;
        
        public ConnectionMultiplexerProvider(Options options)
        {
            if (string.IsNullOrWhiteSpace(options.RedisConnectionString))
            {
                _multiplexer = null;
            }
            else
            {
                _multiplexer = ConnectionMultiplexer.Connect(options.RedisConnectionString);
            }
            
        }

        public ConnectionMultiplexer Get()
        {
            return _multiplexer;
        }
    }
}