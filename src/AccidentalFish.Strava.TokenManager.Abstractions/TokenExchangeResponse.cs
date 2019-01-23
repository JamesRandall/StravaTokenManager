namespace AccidentalFish.Strava.TokenManager.Abstractions
{
    public class TokenExchangeResponse
    {
        public TokenSet TokenSet { get; set; }

        public AthleteSummary AthleteSummary { get; set; }
    }
}
