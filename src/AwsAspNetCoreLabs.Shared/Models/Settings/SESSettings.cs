using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AwsAspNetCoreLabs.Models.Settings
{
    public class SESSettings
    {
        public string FromName { get; set; }
        public string FromAddress { get; set; }
        public string AWSRegion { get; set; }
        public string ConfigurationSet { get; set; }
    }
}
