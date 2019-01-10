// ReSharper disable InconsistentNaming

using AccidentalFish.Strava.TokenManager.Implementation;

namespace AccidentalFish.Strava.TokenManager.Model
{
    internal class TokenExchangeResponse
    {
        public string token_type { get; set; }
            
        public string access_token { get; set; }
            
        public string refresh_token { get; set; }
            
        public long expires_at { get; set; }
            
        public string state { get; set; }
            
        public TokenExchangeAthlete athlete { get; set; }
    }
}