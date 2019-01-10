using System;
using System.Threading.Tasks;
using AccidentalFish.Strava.TokenManager.Abstractions;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace AccidentalFish.Strava.TokenManager.RedisCache.Implementation
{
    internal class TokenCache : ITokenCache
    {
        private readonly IConnectionMultiplexerProvider _connectionMultiplexerProvider;

        public TokenCache(IConnectionMultiplexerProvider connectionMultiplexerProvider)
        {
            _connectionMultiplexerProvider = connectionMultiplexerProvider;
        }

        public async Task SaveTokenSet(string athleteId, string accessToken, string refreshToken, DateTime accessTokenExpiresAtUtc)
        {
            RepositoryEnrichedTokenSet tokenSet = new RepositoryEnrichedTokenSet
            {
                AthleteId = athleteId,
                AccessToken = accessToken,
                ExpiresAtUtc = accessTokenExpiresAtUtc,
                RefreshToken = refreshToken
            };
            string json = JsonConvert.SerializeObject(tokenSet);
            IDatabase redis = _connectionMultiplexerProvider.Get().GetDatabase();
            TimeSpan ttl = tokenSet.ExpiresAtUtc.Subtract(DateTime.UtcNow);
            await redis.StringSetAsync(tokenSet.AccessToken, json, ttl);
        }

        public async Task<RepositoryEnrichedTokenSet> GetForAccessToken(string accessToken)
        {
            IDatabase redis = _connectionMultiplexerProvider.Get().GetDatabase();
            string json = await redis.StringGetAsync(accessToken);
            if (!string.IsNullOrWhiteSpace(json))
            {
                RepositoryEnrichedTokenSet tokenSet = JsonConvert.DeserializeObject<RepositoryEnrichedTokenSet>(json);
                return tokenSet;
            }

            return null;
        }

        public async Task DeleteTokenSetForAccessToken(string accessToken)
        {
            IDatabase database = _connectionMultiplexerProvider.Get().GetDatabase();
            await database.KeyDeleteAsync(accessToken);
        }
    }
}
