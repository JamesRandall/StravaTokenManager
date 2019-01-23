using System;
using System.Collections.Generic;
using System.Text;

namespace AccidentalFish.Strava.TokenManager.Abstractions
{
    public class AthleteSummary
    {
        public string Id { get; set; } // an int currently but we assume it could be anything in the future

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string ProfileImageUrl { get; set; }

        public string EmailAddress { get; set; }

        public string MeasurementPreference { get; set; }
    }
}
