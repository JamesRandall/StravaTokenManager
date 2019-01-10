using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.Strava.TokenManager.Abstractions;
using AccidentalFish.Strava.TokenManager.Implementation;
using AccidentalFish.Strava.TokenManager.Model;
using Flurl.Http.Testing;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace AccidentalFish.Strava.TokenManager.Tests.Implementation.TokenManagerTests
{
    public class TokenExchangeShould : AbstractTokenManagerTest
    {
        [Fact]
        public async Task ContactStravaAndSaveTokens()
        {
            using (var httpTest = new HttpTest())
            {
                const string athleteId = "2";
                const string accessToken = "1";
                const string refreshToken = "4";
                DateTime expiresAt = new DateTime(1970, 1, 1);

                // Arrange
                string tokenResponse = JsonConvert.SerializeObject(new TokenExchangeResponse
                {
                    access_token = accessToken,
                    athlete = new TokenExchangeAthlete { id = athleteId },
                    expires_at = 0,
                    refresh_token = refreshToken,
                    state = "5",
                    token_type = "6"
                });

                httpTest.RespondWith(tokenResponse);

                // Act
                TokenSet result = await _testSubject.TokenExchange("0");

                // Assert
                Assert.Equal(accessToken, result.AccessToken);
                Assert.Equal(expiresAt, result.ExpiresAtUtc);
                Assert.Equal(athleteId, result.AthleteId);
                httpTest.ShouldHaveCalled(_options.TokenEndPoint)
                    .WithVerb(HttpMethod.Post);
                await _tokenCache.Received().SaveTokenSet(athleteId, accessToken, refreshToken, expiresAt);
                await _tokenRepository.Received().SaveTokenSet(athleteId, accessToken, refreshToken, expiresAt);
            }
        }
    }
}
