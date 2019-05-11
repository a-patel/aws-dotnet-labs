namespace AwsAspNetCoreLabs.Models.Settings
{
    public class DynamoDbSessionSettings
    {
        public int IdleTimeoutInMinutes { get; set; }
        public string DynamoDbTableName { get; set; }
        public string DynamoDbRegion { get; set; }
    }
}
