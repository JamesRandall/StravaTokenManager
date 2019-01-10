// ReSharper disable InconsistentNaming
namespace AccidentalFish.Strava.TokenManager.Model
{
    internal class TokenExchangePayload
    {
        public string client_id { get; set; }
            
        public string client_secret { get; set; }
            
        public string code { get; set; }
            
        public string refresh_token { get; set; }
            
        public string grant_type { get; set; }
    }
}