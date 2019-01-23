using System;
using System.Threading.Tasks;
using AccidentalFish.Strava.TokenManager.Abstractions;
using AccidentalFish.Strava.TokenManager.Exceptions;
using AccidentalFish.Strava.TokenManager.Model;
using Flurl.Http;

namespace AccidentalFish.Strava.TokenManager.Implementation
{
    internal class TokenManager : ITokenManager
    {
        private readonly ITokenCache _tokenCache;
        private readonly ITokenRepository _tokenRepository;
        
        private readonly string _tokenEndpoint;

        private readonly string _clientId;

        private readonly string _clientSecret;

        public TokenManager(ITokenCache tokenCache,
            ITokenRepository tokenRepository,
            Options options)
        {
            _tokenCache = tokenCache;
            _tokenRepository = tokenRepository;
            _clientId = options.StravaClientId;
            _clientSecret = options.StravaClientSecret;
            _tokenEndpoint = options.TokenEndPoint;
        }
        
        private async Task SaveTokens(string athleteId, string accessToken, string refreshToken,
            DateTime accessTokenExpiresAtUtc)
        {
            await _tokenRepository.SaveTokenSet(athleteId, accessToken, refreshToken, accessTokenExpiresAtUtc);

            await _tokenCache.SaveTokenSet(athleteId, accessToken, refreshToken, accessTokenExpiresAtUtc);
        }

        public async Task<TokenSet> GetTokenSetForAthleteId(string athleteId, bool attemptRenew)
        {
            RepositoryEnrichedTokenSet savedToken = await _tokenRepository.GetForAthlete(athleteId);
            RepositoryEnrichedTokenSet tokenSet = await CreateTokenSetWithRenewIfRequired(attemptRenew, savedToken);
            return tokenSet;
        }

        public async Task<TokenSet> GetTokenSetForAccessToken(string accessToken, bool attemptRenew)
        {
            // First try the cache
            RepositoryEnrichedTokenSet persistedTokenSet = await _tokenCache.GetForAccessToken(accessToken);
            if (persistedTokenSet == null)
            {
                persistedTokenSet = await _tokenRepository.GetForAccessToken(accessToken);
            }
            RepositoryEnrichedTokenSet result = await CreateTokenSetWithRenewIfRequired(attemptRenew, persistedTokenSet);

            return result;
        }

        private async Task<RepositoryEnrichedTokenSet> CreateTokenSetWithRenewIfRequired(
            bool attemptRenew,
            RepositoryEnrichedTokenSet savedToken)
        {
            if (savedToken == null) throw new TokenSetNotFoundException();

            if (savedToken.ExpiresAtUtc >= DateTime.UtcNow)
            {
                return savedToken;
            }

            if (attemptRenew)
            {
                return await RenewToken(savedToken.AthleteId, savedToken.RefreshToken,
                    savedToken.AccessToken);
            }

            throw new TokenSetExpiredException();
        }

        public async Task<TokenExchangeResponse> TokenExchange(string code)
        {
            TokenExchangePayload payload = new TokenExchangePayload
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                code = code,
                grant_type = "authorization_code"
            };
            
            StravaTokenExchangeResponse stravaTokenResponse = await _tokenEndpoint
                .PostJsonAsync(payload)
                .ReceiveJson<StravaTokenExchangeResponse>();

            await SaveTokens(stravaTokenResponse.athlete.id, stravaTokenResponse.access_token, stravaTokenResponse.refresh_token,
                ExpiresAtUtc(stravaTokenResponse));

            return new TokenExchangeResponse
            {
                TokenSet = new TokenSet
                {
                    AthleteId = stravaTokenResponse.athlete.id,
                    AccessToken = stravaTokenResponse.access_token,
                    ExpiresAtUtc = ExpiresAtUtc(stravaTokenResponse)
                },
                AthleteSummary = new AthleteSummary
                {
                    EmailAddress = stravaTokenResponse.athlete?.email,
                    FirstName = stravaTokenResponse.athlete?.firstname,
                    Id = stravaTokenResponse.athlete?.id,
                    LastName = stravaTokenResponse.athlete?.lastname,
                    MeasurementPreference = stravaTokenResponse.athlete?.measurement_preference,
                    ProfileImageUrl = stravaTokenResponse.athlete?.profile
                }                
            };
        }

        private static DateTime ExpiresAtUtc(StravaTokenExchangeResponse stravaTokenResponse)
        {
            // strava returns this in seconds rather than the milliseconds in JavaScript's getTime()
            return new DateTime(1970, 01, 01).AddSeconds(stravaTokenResponse.expires_at);
        }

        private async Task<RepositoryEnrichedTokenSet> RenewToken(string athleteId, string refreshToken, string existingAccessToken)
        {
            TokenExchangePayload payload = new TokenExchangePayload
            {
                client_id = _clientId,
                client_secret = _clientSecret,
                refresh_token = refreshToken,
                grant_type = "refresh_token"
            };

            StravaTokenExchangeResponse stravaTokenResponse = await _tokenEndpoint
                .PostJsonAsync(payload)
                .ReceiveJson<StravaTokenExchangeResponse>();
            
            await SaveTokens(athleteId, stravaTokenResponse.access_token, stravaTokenResponse.refresh_token,
                ExpiresAtUtc(stravaTokenResponse));
           
            await DeleteOldAccessToken(existingAccessToken);

            return new RepositoryEnrichedTokenSet
            {
                AccessToken = stravaTokenResponse.access_token,
                ExpiresAtUtc = ExpiresAtUtc(stravaTokenResponse),
                AthleteId = athleteId,
                RefreshToken = stravaTokenResponse.refresh_token
            };
        }

        private async Task DeleteOldAccessToken(string existingAccessToken)
        {
            await _tokenRepository.DeleteTokenSetForAccessToken(existingAccessToken);

            await _tokenCache.DeleteTokenSetForAccessToken(existingAccessToken);
        }
    }
}