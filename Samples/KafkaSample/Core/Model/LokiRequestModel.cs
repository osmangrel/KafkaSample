using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Model
{
    [Serializable]
    public class LokiRequestModel
    {
        public LokiRequestModel(string data)
        {
            this.streams = new List<Stream>();
            this.streams.Add(
                    new Stream()
                    {
                        labels = "{source=\"JSON\",job=\"SendLogToKafka\", host=\"LogConsumerMS\"}",
                        entries = new List<Entry>() { new Entry() { ts = DateTime.Now, line = data } }
                    }
                );
        }
        public List<Stream> streams { get; set; }
        public string getNanoSecond()
        {
            DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return ((DateTime.UtcNow - epochStart).Ticks * 100).ToString();
        }

    }
    [Serializable]
    public class Stream
    {
        public string labels { get; set; }
        public List<Entry> entries { get; set; }
    }
    [Serializable]
    public class Entry
    {
        public DateTime ts { get; set; }
        public string line { get; set; }
    }
}
