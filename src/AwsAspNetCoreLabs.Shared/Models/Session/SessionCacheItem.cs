namespace AwsAspNetCoreLabs.Models.Session
{
    public class SessionCacheItem
    {
        public string CacheId { get; set; }

        public string Value { get; set; }

        public long Ttl { get; set; }

        public SessionCacheOptions CacheOptions { get; set; }
    }
}
