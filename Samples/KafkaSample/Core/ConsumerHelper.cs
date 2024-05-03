using Confluent.Kafka;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core
{
    public class ConsumerHelper
    {

        public static async Task<string> KafkaConsumeTopic(string serverUrl, IEnumerable<string> topics, string consumerGroupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = serverUrl,
                GroupId = consumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topics);
                //Console.WriteLine($"Consumer Çalışmaya başladı. Topic: {topic}");
                string topicsString = string.Join(", ", topics);
                Console.WriteLine($"Consumer Çalışmaya başladı. Topic: {topicsString}");

                while (true)
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);
                    bool isWriteToConsole = false;

                    if (consumeResult != null)
                        isWriteToConsole = await WriteToConsole(consumeResult.Topic, consumeResult.Message.Value);

                    if (isWriteToConsole)
                        consumer.Commit();
                }
            }
        }

        public static async Task KafkaConsumeTopicSendLoki(string serverUrl, IEnumerable<string> topics, string consumerGroupId)
        {
            var config = new ConsumerConfig
            {
                BootstrapServers = serverUrl,
                GroupId = consumerGroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = false
            };

            using (var consumer = new ConsumerBuilder<Ignore, string>(config).Build())
            {
                consumer.Subscribe(topics);
                string topicsString = string.Join(", ", topics);
                Console.WriteLine($"Consumer Çalışmaya başladı. Topic: {topicsString}");

                while (true)
                {
                    var consumeResult = consumer.Consume(CancellationToken.None);
                    bool isSendToLoki = false;

                    if (consumeResult != null)
                    {
                        isSendToLoki = await LokiHelper.SendLog(consumeResult.Message.Value);
                        await WriteToConsole(consumeResult.Topic, consumeResult.Message.Value);
                    }
                    else
                    {
                        consumer.Commit();
                    }

                    if (isSendToLoki)
                        consumer.Commit();
                }
            }
        }


        public static async Task<bool> WriteToConsole(string topic, string data)
        {
            Console.WriteLine($"Topic:{topic} - Tüketilen mesaj: {data}");
            await Task.Delay(500);
            return true;
        }


    }
}
