using System;
using System.Text;

namespace AccidentalFish.Strave.TokenManager.AzureTableStorageTokenStore.Extensions
{
    internal static class StringExtensions
    {
        public static string EncodeForTableEntityKey(this string value)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(value);
            string base64 = Convert.ToBase64String(keyBytes);
            return base64.Replace('/', '_');
        }

        public static string DecodeFromTableEntityKey(this string value)
        {
            var base64 = value.Replace('_', '/');
            byte[] bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }
    }
}