namespace AwsAspNetCoreLabs.Models.Logging
{
    public class LogExceptionModel
    {
        public string Source { get; set; }
        public string Message { get; set; }
        public string[] StackTrace { get; set; }
        public LogExceptionModel InnerException { get; set; }
    }
}
