using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace vaxalert.Stores
{
    [DataContract]
    public class SourceScan
    {
        public string Source { get; set; }
        
        [JsonProperty(PropertyName = "id")]
        public Guid Id { get; set; }
        
        [JsonProperty]
        public DateTime Time { get; set; }
    }
}