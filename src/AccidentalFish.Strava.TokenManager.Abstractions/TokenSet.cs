using System;

namespace AccidentalFish.Strava.TokenManager.Abstractions
{
    /// <summary>
    /// Strava token set
    /// </summary>
    public class TokenSet
    {
        /// <summary>
        /// The athlete ID
        /// </summary>
        public string AthleteId { get; set; }
        
        /// <summary>
        /// The access token
        /// </summary>
        public string AccessToken { get; set; }
        
        /// <summary>
        /// The expiry date and time of the access token
        /// </summary>
        public DateTime ExpiresAtUtc { get; set; }
    }
}