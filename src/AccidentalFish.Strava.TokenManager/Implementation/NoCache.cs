using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.Strava.TokenManager.Abstractions;

namespace AccidentalFish.Strava.TokenManager.Implementation
{
    class NoCache : ITokenCache
    {
        public Task SaveTokenSet(string athleteId, string accessToken, string refreshToken, DateTime accessTokenExpiresAtUtc)
        {
            return Task.CompletedTask;
        }

        public Task<RepositoryEnrichedTokenSet> GetForAccessToken(string accessToken)
        {
            return Task.FromResult<RepositoryEnrichedTokenSet>(null);
        }

        public Task DeleteTokenSetForAccessToken(string accessToken)
        {
            return Task.FromResult<RepositoryEnrichedTokenSet>(null);
        }
    }
}
