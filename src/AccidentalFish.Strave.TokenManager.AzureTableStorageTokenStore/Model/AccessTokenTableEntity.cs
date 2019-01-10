using System;
using AccidentalFish.Strave.TokenManager.AzureTableStorageTokenStore.Extensions;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Strave.TokenManager.AzureTableStorageTokenStore.Model
{
    internal class AccessTokenTableEntity : TableEntity
    {
        public string AthleteId { get; set; }
        
        public string AccessToken { get; set; }
        
        public string RefreshToken { get; set; }
        
        public DateTime RefreshTokenObtainedAtUtc { get; set; }
        
        public DateTime AccessTokenExpiryUtc { get; set; }

        public static string GetPartitionKeyByAthleteId(string atheleteId)
        {
            return atheleteId.ToString();
        }

        public static string GetRowKeyByAthleteId()
        {
            return String.Empty;
        }

        public static string GetPartitionKeyByAccessToken(string accessToken)
        {
            return accessToken.EncodeForTableEntityKey();
        }

        public static string GetRowKeyByAccessToken()
        {
            return String.Empty;
        }
    }
}