namespace AwsAspNetCoreLabs.Models.Settings
{
    public class S3Settings
    {
        public string BucketName { get; set; }
        public string AWSRegion { get; set; }
        public string AWSAccessKey { get; set; }
        public string AWSSecretKey { get; set; }
    }
}
