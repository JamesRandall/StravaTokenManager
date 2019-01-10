namespace AccidentalFish.Strava.TokenManager.AzureTableStorageTokenStore
{
    public class Options
    {
        public Options()
        {
            ByAccessTokenTableName = "tokensbyaccesstoken";
            ByAthleteIdTableName = "tokensbyathleteid";
        }

        /// <summary>
        /// The storage account connection string. Required.
        /// </summary>
        public string StorageAccountConnectionString { get; set; }

        /// <summary>
        /// The name of the table that saves access tokens indexed by access token. Defaults to tokensbyaccesstoken.
        /// </summary>
        public string ByAccessTokenTableName { get; set; }

        /// <summary>
        /// The name of the table that saves access tokens indexed by athlete ID. Detaults to tokensbyathleteid
        /// </summary>
        public string ByAthleteIdTableName { get; set; }
    }
}
