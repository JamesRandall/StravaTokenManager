# Strava Token Manager

![Master build status](https://accidentalfish.visualstudio.com/Strava%20Token%20Manager/_apis/build/status/Build%20and%20Test?branchName=master) Master

![Production build status](https://accidentalfish.visualstudio.com/Strava%20Token%20Manager/_apis/build/status/Build%20and%20Test?branchName=production) Production

_Before reading this guide it is highly recommended you read the [Strava authentication documentation](https://developers.strava.com/docs/authentication/)._

This package can help you manage the server side part of the Strava authentication flow. Its functionality is accessed by the _ITokenManager_ interface made available through depedendency injection.

## Getting Started

First install two NuGet packages:

    Install-Package AccidentalFish.Strava.TokenManager
    Install-Package AccidentalFish.Strava.TokenManager.AzureTableTokenStore

To register the dependencies use the _UseStravaTokenManager_ extension method on the _IServiceCollection_ interface, for example:

    IServiceCollection collection = ...; // typically provided by a framework startup system e.g. ASP. Net Core
    collection.UseStravaTokenManager(new AccidentalFish.Strava.TokenManager.Options {
        StravaClientId = Environment.GetEnvironmentVariable("stravaClientId"),
        StravaClientSecret = Environment.GetEnvironmentVariable("stravaClientSecret") // or key vault!
    });

_In a future version the following step will be optional but for the moment the token manager stores a refresh token and will use it to obtain new access tokens upon access token expiry._

You will also need to register a persistent store for tokens:

    collection.UseAzureTableStorageTokenRepository(
        new AccidentalFish.Strava.TokenManager.AzureTableStorageTokenStore.Options
        {
            StorageAccountConnectionString = Environment.GetEnvironmentVariable("your-storage-connection-string")
        });

## Token Exchange - Obtaining an access token

After a user has completed the Strava authentication process Strava will redirect to your application and give you a code. This code needs to be sent back to Strava with your client ID and secret to obtain an access token that you can then use to call Strava APIs. The _TokenExchange_ method on the _ITokenManager_ interface can handle this process for you, the following simple class shows the _ITokenManager_ being injected and the code being used to obtain an access token:

    class MyTokenService
    {
        private readonly ITokenManager _tokenManager;

        public MyTokenService(ITokenManager tokenManager)
        {
            _tokenManager = tokenManager;
        }

        public async Task<string> GetAccessTokenFromCode(string code)
        {
            TokenSet tokenSet = await _tokenManager.TokenExchange(code);
            return tokenSet.AccessCode;
        }
    }

The TokenSet class will also has properties that contain the expiry date and time of the access token (_ExpiresAtUtc_) and the athletes Strava ID (_AthleteId_).

## Validating an access token

Typically you will want to ensure that an access token is valid (for example during a REST API call from a client) and you can do this by attempting to retrieve the _TokenSet_ for an access token through the _ITokenManager_ interface:

    public Task<bool> VerifyAccessToken(string accessToken)
    {
        TokenSet tokenSet = await _tokenManager.GetTokenSetForAccessToken(accessToken, false);
        Console.Log($"Token validated for athlete {tokenSet.AthleteId}");
        return true;
    }

If the token is invalid for some reason then an exception will be thrown by _GetTokenSetForAccessToken_.

The second parameter of _GetTokenSetForAccessToken_ indicates to the token manager if the refresh token should be used to renew the access token if it is out of date. This will return a new access token in the _TokenSet_ that you will need to use on subsequent calls. If it is false token renewal will not be attempted.

If you need to obtain a token during a background process then you can do so using the _GetTokenSetForAthleteId_ method - this behaves in exactly the same way as the _GetTokenSetForAccessToken_ method but accepts an athlete ID rather than an access token.

Note that the refresh token is never returned from the token manager - due to its long life and capability to create a new access token this is a potentially sensitive token, it is therefore not returned to mitigate leakage. If you do need access to the refresh token you can obtain it through the _ITokenRepository_ interface (dependency injectable).

## Caching of tokens

In a REST API implementation the _GetTokenSetForAccessToken_ call will be called on every REST call and so there is significant benefit in making this a cheap operation. The NuGet package _AccidentalFish.Strava.TokenManager.Abstractions_ exposes an _ITokenCache_ interface that you can use to implement a cache (the methods are fairly self explanatory). An already built Redis based cache is supplied in the _AccidentalFish.Strave.TokenManager.RedisCache_ package. To use this first add the package:

    Install-Package AccidentalFish.Strava.TokenManager

And then register it with the _IServiceCollection_:

    collection.UseRedisTokenCache(new AccidentalFish.Strava.TokenManager.RedisCache.Options
    {
        RedisConnectionString = Environment.GetEnvironmentVariable("redis-connection-string")
    });

## Implementing your own token storage repository

The _AccidentalFish.Strava.TokenManager.Abstractions_ NuGet package exposes an _ITokenRepository_ interface that can be implemented to provide a storage mechanism of your choice. The methods on this interface are fairly self explanatory.

## Code Status

There are a few improvements I'd like to make and will over time but fundamentally the token manager works in simple projects and it has a set of unit tests. If you come across any issues then please reach out to me on [Twitter](https://twitter.com/AzureTrenches) or use the GitHub Issues here.
