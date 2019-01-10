using System;
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
    public class RenewTokenShould : AbstractTokenManagerTest
    {
        [Fact]
        public async Task SaveToken()
        {
            using (var httpTest = new HttpTest())
            {
                // Arrange
                RepositoryEnrichedTokenSet tokenSet = new RepositoryEnrichedTokenSet
                {
                    AthleteId = "1",
                    AccessToken = "2",
                    ExpiresAtUtc = DateTime.UtcNow.AddDays(-1),
                    RefreshToken = "3"
                };
                _tokenRepository.GetForAthlete("1").Returns(tokenSet);
                httpTest.RespondWith(JsonConvert.SerializeObject(new TokenExchangeResponse
                {
                    access_token = "4",
                    athlete = new TokenExchangeAthlete { id = "1" },
                    expires_at = (int)DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                    refresh_token = "5"
                }));

                // Act
                await _testSubject.GetTokenSetForAthleteId("1", true);

                // Assert
                await _tokenCache.Received().SaveTokenSet("1", "4", "5", Arg.Any<DateTime>());
                await _tokenRepository.Received().SaveTokenSet("1", "4", "5", Arg.Any<DateTime>());
            }
        }
    }
}
