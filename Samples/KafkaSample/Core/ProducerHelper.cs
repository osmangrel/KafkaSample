using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ProducerHelper
    {
        public static async Task<DeliveryResult<Null, string>> SendKafkaData(string serverUrl, string topic, string data)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = serverUrl
            };

            using (var producer = new ProducerBuilder<Null, string>(config).Build())
            {
                try
                {
                    var deliveryReport = await producer.ProduceAsync(topic, new Message<Null, string> { Value = data });
                    return deliveryReport;
                }
                catch (ProduceException<Null, string> e)
                {
                    Console.WriteLine($"Bir hata oluştu: {e.Error.Reason}");
                    return null;
                }
            }
        }
    }
}
