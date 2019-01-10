using System;
using System.Collections.Generic;
using System.Text;

namespace AccidentalFish.Strava.TokenManager.Exceptions
{
    public class TokenSetNotFoundException : Exception
    {
        public TokenSetNotFoundException() : base("The token set could not be found") { }
    }
}
