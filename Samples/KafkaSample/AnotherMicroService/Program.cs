using Bogus;
using Confluent.Kafka;
using Core;
using Core.Model;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace AnotherMicroService
{
    public class Program
    {
        private static readonly string kafkaUrl = "localhost:9092";
        private static readonly string topic = "ms-logs";
        static async Task Main(string[] args)
        {
            Console.WriteLine("Another Microservice Başlatıldı.");

            while (true)
            {
                var logModel = new Faker<LogModel>().
                                RuleFor(l => l.log, f => f.Lorem.Sentence(10)).
                                RuleFor(l => l.LogLevel, f => f.PickRandom<LogLevel>()).
                                RuleFor(l => l.createDate, f => DateTime.Now).Generate();

                //SendKafkaToLog
                string content = JsonSerializer.Serialize(logModel);
                await ProducerHelper.SendKafkaData(kafkaUrl, topic, content);
                Console.WriteLine($"     Kafka'ya gönderilen Log:\n {content}");
                await Task.Delay(100);
            }
        }
    }
}
