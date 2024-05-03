using Confluent.Kafka;
using Core;
using Core.Model;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Formatting.Display;
using Serilog.Sinks.Grafana.Loki;
using System.Diagnostics;
using System.Text.Json;

namespace LogConsumerMs
{
    internal class Program
    {
        private static readonly IEnumerable<string> topics = new string[] { "ms-logs" };
        private static readonly string kafkaUrl = "localhost:9092";
        private static readonly string consumerGroup = "ms-logs-consumers";


        static async Task Main(string[] args)
        {
            HandleApplicationCloseEvent();

            Serilog.ILogger logger = new LoggerConfiguration().
                               Enrich.WithProperty("MachineName", Environment.MachineName).
                               WriteTo.
                               GrafanaLoki(
                                    "http://localhost:3100",
                                    labels: new List<LokiLabel>
                                    {
                                        new() {Key = "job", Value = "LogConsumerMs"},
                                        new() {Key = "source", Value = "csharp_backend"}
                                    },
                                    textFormatter: new LokiJsonTextFormatter()
                               ).
                               CreateLogger();

            //await ConsumerHelper.KafkaConsumeTopicSendLoki(kafkaUrl, topics, consumerGroup);
            var config = new ConsumerConfig
            {
                BootstrapServers = kafkaUrl,
                GroupId = consumerGroup,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false,
                ClientId=Guid.NewGuid().ToString(),
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topics);
                string topicsString = string.Join(", ", topics);
                Console.WriteLine($"Consumer Çalışmaya başladı. Topic: {topicsString}");

                while (true)
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);

                    if (consumeResult != null)
                    {
                        WriteToLokiWithSerilog(consumeResult.Message.Value, consumeResult.Topic, logger);
                        consumer.Commit();
                    }
                }
            }

        }

        static void WriteToLokiWithSerilog(string logMessage, string topic, Serilog.ILogger logger)
        {
            try
            {
                var logModel = JsonSerializer.Deserialize<LogModel>(logMessage);
                //var reqModel = new LokiRequestModel(logMessage);
                //var content = JsonSerializer.Serialize(reqModel);
                switch (logModel.LogLevel)
                {
                    case LogLevel.Trace:
                        logger.Verbose(logMessage);
                        break;
                    case LogLevel.Debug:
                        logger.Debug(logMessage);
                        break;
                    case LogLevel.Information:
                        logger.Information(logMessage);
                        break;
                    case LogLevel.Warning:
                        logger.Warning(logMessage);
                        break;
                    case LogLevel.Error:
                        logger.Error(logMessage);
                        break;
                    case LogLevel.Critical:
                        logger.Fatal(logMessage);
                        break;
                    default:
                        break;
                }
                Console.WriteLine($" Topic:{topic} - Tüketilen mesaj: \n{logMessage}");
            }
            catch (Exception)
            {

                throw;
            }
           

        }

        static void HandleApplicationCloseEvent()
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                //eventArgs.Cancel = true;
                Console.WriteLine("Uygulama kapatılmaya çalışıldı. Çıkış işlemi yapılıyor.");
                Log.CloseAndFlush();
                Environment.Exit(0);


            };
            AppDomain.CurrentDomain.ProcessExit += (sender, eventArgs) =>
            {
                Console.WriteLine("Uygulama kapatılmaya çalışıldı. Çıkış işlemi yapılıyor.");
                Log.CloseAndFlush();
                Environment.Exit(0);

            };
        }

    }
}
