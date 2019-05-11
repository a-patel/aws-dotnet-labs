using Newtonsoft.Json;

namespace AwsAspNetCoreLabs.Models.Logging
{
    public class LogEventModel
    {
        private static readonly JsonSerializerSettings JsonSerializerSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        public string Logger { get; set; }
        public string Level { get; set; }
        public string MachineName { get; set; }
        public string Message { get; set; }
        public LogExceptionModel Exception { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this, JsonSerializerSettings);
        }
    }
}
