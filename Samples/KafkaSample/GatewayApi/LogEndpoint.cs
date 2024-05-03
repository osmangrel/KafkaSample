

using Bogus;
using Core;
using Core.Model;
using System.Text.Json;

namespace GatewayApi
{
    public class LogEndpoint
    {
        private static readonly string kafkaUrl = "localhost:9092";
        private static readonly string topic = "ms-logs";

        public static async Task<IResult> AddLogRandom()
        {
            var logModel = new Faker<LogModel>().
                               RuleFor(l => l.log, f => f.Lorem.Sentence(10)).
                               RuleFor(l => l.LogLevel, f => f.PickRandom<LogLevel>()).
                               RuleFor(l => l.createDate, f => DateTime.Now).Generate();

            if (logModel == null) return Results.NotFound();

            //SendKafkaToLog
            string content = JsonSerializer.Serialize(logModel);
            await ProducerHelper.SendKafkaData(kafkaUrl, topic, content);

            return Results.Ok(logModel);
        }

    }
}
