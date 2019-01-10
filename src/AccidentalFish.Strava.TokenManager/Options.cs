namespace AccidentalFish.Strava.TokenManager
{
    public class Options
    {
        /// <summary>
        /// The client ID of your Strava app
        /// </summary>
        public string StravaClientId { get; set; }

        /// <summary>
        /// The client secret of your Strava app
        /// </summary>
        public string StravaClientSecret { get; set; }

        /// <summary>
        /// The Strava token endpoint defaults to https://www.strava.com/oauth/token
        /// </summary>
        public string TokenEndPoint { get; set; } = "https://www.strava.com/oauth/token";
    }
}
