using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AccidentalFish.Strava.TokenManager.Abstractions;
using AccidentalFish.Strava.TokenManager.Exceptions;
using AccidentalFish.Strava.TokenManager.Implementation;
using AccidentalFish.Strava.TokenManager.Model;
using Flurl.Http.Testing;
using Newtonsoft.Json;
using NSubstitute;
using Xunit;

namespace AccidentalFish.Strava.TokenManager.Tests.Implementation.TokenManagerTests
{
    public class GetTokenSetForAccessTokenShould : AbstractTokenManagerTest
    {
        [Fact]
        public async Task ThrowExceptionWhenTokenNotFound()
        {
            await Assert.ThrowsAsync<TokenSetNotFoundException>(() => _testSubject.GetTokenSetForAccessToken("2", false));
        }

        [Fact]
        public async Task ThrowExceptionWhenExpiredAndNoRenewal()
        {
            // Arrange
            RepositoryEnrichedTokenSet tokenSet = new RepositoryEnrichedTokenSet
            {
                AthleteId = "1",
                AccessToken = "2",
                ExpiresAtUtc = DateTime.UtcNow.AddDays(-1),
                RefreshToken = "3"
            };
            _tokenRepository.GetForAccessToken("2").Returns(tokenSet);

            // Act
            await Assert.ThrowsAsync<TokenSetExpiredException>(() => _testSubject.GetTokenSetForAccessToken("2", false));
        }

        [Fact]
        public async Task ReturnValidTokenWhenFound()
        {
            // Arrange
            RepositoryEnrichedTokenSet tokenSet = new RepositoryEnrichedTokenSet
            {
                AthleteId = "1",
                AccessToken = "2",
                ExpiresAtUtc = DateTime.UtcNow.AddDays(1),
                RefreshToken = "3"
            };
            _tokenRepository.GetForAccessToken("2").Returns(tokenSet);

            // Act
            TokenSet result = await _testSubject.GetTokenSetForAccessToken("2", false);

            // Assert
            Assert.Equal("1", result.AthleteId);
            Assert.Equal("2", result.AccessToken);
        }

        [Fact]
        public async Task ReturnValidTokenWhenExpiredAndRenewAllowed()
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
                _tokenRepository.GetForAccessToken("1").Returns(tokenSet);
                httpTest.RespondWith(JsonConvert.SerializeObject(new StravaTokenExchangeResponse
                {
                    access_token = "4",
                    athlete = new TokenExchangeAthlete { id = "1" },
                    expires_at = (int)DateTime.UtcNow.AddDays(1).Subtract(new DateTime(1970, 1, 1)).TotalSeconds,
                    refresh_token = "5"
                }));

                // Act
                TokenSet result = await _testSubject.GetTokenSetForAccessToken("1", true);

                // Assert
                Assert.Equal("1", result.AthleteId);
                Assert.Equal("4", result.AccessToken);
                httpTest.ShouldHaveCalled(_options.TokenEndPoint);

            }
        }
    }
}
