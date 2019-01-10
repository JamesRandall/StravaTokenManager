namespace AccidentalFish.Strava.TokenManager.Model
{
    internal class TokenExchangeAthlete
    {
        public string id { get; set; } // an int currently but we assume it could be anything in the future
        
        public string firstname { get; set; }
        
        public string lastname { get; set; }
        
        public string profile { get; set; }
        
        public string email { get; set; }
       
    }
}