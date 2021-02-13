using System;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace vaxalert.Controllers
{
    [DataContract]
    public class Appointment :  Microsoft.Azure.Documents.Resource, IEquatable<Appointment>
    {
        public bool Equals(Appointment other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Key == other.Key;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Appointment) obj);
        }

        public override int GetHashCode()
        {
            return (Key != null ? Key.GetHashCode() : 0);
        }

        public Appointment() { }
        
        public Appointment(
            string source,
            string location,
            string title,
            DateTime? date = null,
            DateTime? time = null,
            string notes = null)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Location = location ?? throw new ArgumentNullException(nameof(location));
            Title = title ?? throw new ArgumentNullException(nameof(title));
            Date = date;
            Time = time;
            Notes = notes;
        }
        
        [JsonProperty]
        public string Source { get; set; }

        [JsonProperty]
        public string Location { get; set; }
        
        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty]
        public DateTime? Date { get; set; }
        
        [JsonProperty]
        public DateTime? Time { get; set; }
        
        [JsonProperty]
        public string Notes { get; set; }

        [JsonProperty]
        public bool Available { get; set; }

        [JsonProperty]
        public string Key => $"{Location}:{Title}:{Date}:{Time}";
        
        [JsonProperty]
        public Guid ScanId { get; set; }
        
        [JsonProperty]
        public DateTime ScanTime { get; set; }
    }
}