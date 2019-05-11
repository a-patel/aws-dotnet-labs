namespace AwsAspNetCoreLabs.Models.Session
{
    public class SessionCacheOptions
    {
        public CacheExpiryType Type { get; set; }
        public long Span { get; set; }
    }
}