using AccidentalFish.Strava.TokenManager.Abstractions;
using NSubstitute;

namespace AccidentalFish.Strava.TokenManager.Tests.Implementation.TokenManagerTests
{
    public abstract class AbstractTokenManagerTest
    {
        protected readonly ITokenCache _tokenCache = Substitute.For<ITokenCache>();

        protected readonly ITokenRepository _tokenRepository = Substitute.For<ITokenRepository>();

        protected readonly Options _options = new Options()
        {
            StravaClientId = "1234",
            StravaClientSecret = "5678"
        };

        protected readonly ITokenManager _testSubject;

        protected AbstractTokenManagerTest()
        {
            _testSubject = new TokenManager.Implementation.TokenManager(_tokenCache,
                _tokenRepository,
                _options);
        }
    }
}
