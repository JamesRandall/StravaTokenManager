using System;
using System.Threading.Tasks;
using AccidentalFish.Strava.TokenManager.Abstractions;
using AccidentalFish.Strava.TokenManager.AzureTableStorageTokenStore.Model;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace AccidentalFish.Strava.TokenManager.AzureTableStorageTokenStore.Implementation
{
    internal class TokenRepository : ITokenRepository
    {
        private readonly CloudTable _byAtheleteIdTable;
        private readonly CloudTable _byAccessTokenTable;

        public TokenRepository(Options options)
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(options.StorageAccountConnectionString);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            _byAccessTokenTable = tableClient.GetTableReference(options.ByAccessTokenTableName);
            _byAtheleteIdTable = tableClient.GetTableReference(options.ByAthleteIdTableName);
        }

        public async Task SaveTokenSet(string athleteId, string accessToken, string refreshToken, DateTime accessTokenExpiresAtUtc)
        {
            AccessTokenTableEntity byAthelete = new AccessTokenTableEntity
            {
                AccessToken = accessToken,
                AccessTokenExpiryUtc = accessTokenExpiresAtUtc,
                AthleteId = athleteId,
                PartitionKey = AccessTokenTableEntity.GetPartitionKeyByAthleteId(athleteId),
                RefreshToken = refreshToken,
                RefreshTokenObtainedAtUtc = DateTime.UtcNow,
                RowKey = AccessTokenTableEntity.GetRowKeyByAthleteId()
            };
            AccessTokenTableEntity byAccessToken = new AccessTokenTableEntity
            {
                AccessToken = accessToken,
                AccessTokenExpiryUtc = accessTokenExpiresAtUtc,
                AthleteId = athleteId,
                PartitionKey = AccessTokenTableEntity.GetPartitionKeyByAccessToken(accessToken),
                RefreshToken = refreshToken,
                RefreshTokenObtainedAtUtc = DateTime.UtcNow,
                RowKey = AccessTokenTableEntity.GetRowKeyByAccessToken()
            };

            await Task.WhenAll(
                _byAtheleteIdTable.ExecuteAsync(TableOperation.InsertOrReplace(byAthelete)),
                _byAccessTokenTable.ExecuteAsync(TableOperation.InsertOrReplace(byAccessToken))
            );
        }

        public async Task<RepositoryEnrichedTokenSet> GetForAthlete(string athleteId)
        {
            TableResult tableResult = await _byAtheleteIdTable.ExecuteAsync(TableOperation.Retrieve<AccessTokenTableEntity>(
                AccessTokenTableEntity.GetPartitionKeyByAthleteId(athleteId),
                AccessTokenTableEntity.GetRowKeyByAthleteId()));
            if (tableResult != null)
            {
                AccessTokenTableEntity tableEntity = (AccessTokenTableEntity) tableResult.Result;
                return CreatedEnrichedTokenSet(tableEntity);
            }

            return null;
        }

        public async Task<RepositoryEnrichedTokenSet> GetForAccessToken(string accessToken)
        {
            TableResult tableResult = await _byAccessTokenTable.ExecuteAsync(TableOperation.Retrieve<AccessTokenTableEntity>(
                AccessTokenTableEntity.GetPartitionKeyByAccessToken(accessToken),
                AccessTokenTableEntity.GetRowKeyByAccessToken()));
            if (tableResult != null)
            {
                AccessTokenTableEntity tableEntity = (AccessTokenTableEntity)tableResult.Result;
                return CreatedEnrichedTokenSet(tableEntity);
            }

            return null;
        }

        public async Task DeleteTokenSetForAccessToken(string existingAccessToken)
        {
            TableResult byAccessTokenTableResult = await _byAtheleteIdTable.ExecuteAsync(TableOperation.Retrieve<AccessTokenTableEntity>(
                AccessTokenTableEntity.GetPartitionKeyByAccessToken(existingAccessToken),
                AccessTokenTableEntity.GetRowKeyByAccessToken()));
            if (byAccessTokenTableResult != null)
            {
                AccessTokenTableEntity byAccessTokenTableEntity = (AccessTokenTableEntity)byAccessTokenTableResult.Result;
                TableResult byAthleteTableResult = await _byAtheleteIdTable.ExecuteAsync(TableOperation.Retrieve<AccessTokenTableEntity>(
                    AccessTokenTableEntity.GetPartitionKeyByAthleteId(byAccessTokenTableEntity.AthleteId),
                    AccessTokenTableEntity.GetRowKeyByAthleteId()));

                if (byAthleteTableResult != null)
                {
                    AccessTokenTableEntity byAthleteTableEntity = (AccessTokenTableEntity) byAthleteTableResult.Result;
                    await _byAtheleteIdTable.ExecuteAsync(TableOperation.Delete(byAthleteTableEntity));
                }

                await _byAccessTokenTable.ExecuteAsync(TableOperation.Delete(byAccessTokenTableEntity));
            }
        }

        private static RepositoryEnrichedTokenSet CreatedEnrichedTokenSet(AccessTokenTableEntity tableEntity)
        {
            return new RepositoryEnrichedTokenSet
            {
                AthleteId = tableEntity.AthleteId,
                AccessToken = tableEntity.AccessToken,
                ExpiresAtUtc = tableEntity.AccessTokenExpiryUtc,
                RefreshToken = tableEntity.RefreshToken
            };
        }
    }
}
