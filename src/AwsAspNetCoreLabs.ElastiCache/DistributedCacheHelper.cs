using AwsAspNetCoreLabs.Models.Session;
using Microsoft.Extensions.Caching.Distributed;
using System;

namespace AwsAspNetCoreLabs.ElastiCache
{
    public static class DistributedCacheHelper
    {
        public static long ToEpochTime(DateTime date)
        {
            var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return Convert.ToInt64((date - epoch).TotalSeconds);
        }

        public static long ToTtl(long expiryDuration)
        {
            return ToEpochTime(DateTime.UtcNow.AddMinutes(expiryDuration));
        }

        public static CacheExpiryType GetCacheExpiryType(DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration != null || options.AbsoluteExpirationRelativeToNow != null)
            {
                return CacheExpiryType.Absolute;
            }
            else if (options.SlidingExpiration != null)
            {
                return CacheExpiryType.Sliding;
            }

            throw new ArgumentException("Cache expiry strategy not set");
        }

        public static long GetCacheExpiryDurationInMinutes(DistributedCacheEntryOptions options)
        {
            if (options.AbsoluteExpiration != null)
            {
                return (long)(options.AbsoluteExpiration.Value.ToUniversalTime().DateTime - DateTimeOffset.UtcNow.DateTime).TotalMinutes;
            }

            if (options.AbsoluteExpirationRelativeToNow != null)
            {
                return (long)options.AbsoluteExpirationRelativeToNow.Value.TotalMinutes;
            }

            if (options.SlidingExpiration != null)
            {
                return (long)options.SlidingExpiration.Value.TotalMinutes;
            }

            throw new ArgumentException("Cache expiry strategy not set");
        }
    }
}
