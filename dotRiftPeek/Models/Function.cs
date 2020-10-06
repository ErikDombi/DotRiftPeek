using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotRiftPeek.Models
{
    public class Function
    {
        [JsonProperty("arguments")]
        public object[] Arguments;

        [JsonProperty("http_method")]
        public string HttpMethod;

        [JsonProperty("returns")]
        public object Returns;

        [JsonProperty("url")]
        public string url;

        [JsonProperty("usage")]
        public string usage;

        [JsonIgnore]
        public string paramUrl => url + string.Join("/", usage.Split(" ").Skip(1));
    }
}
