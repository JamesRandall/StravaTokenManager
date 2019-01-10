using System;
using System.Collections.Generic;
using System.Text;

namespace AccidentalFish.Strava.TokenManager.Exceptions
{
    public class TokenSetExpiredException : Exception
    {
        public TokenSetExpiredException() : base("The token has expired") { }
    }
}
