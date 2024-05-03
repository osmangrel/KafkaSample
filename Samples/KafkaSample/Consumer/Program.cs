using Core;

namespace Consumer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            IEnumerable<string> topics = new string[] {"sample-one","sample-two"};
            await ConsumerHelper.KafkaConsumeTopic("localhost:9092",topics , "sample1-consumer-group");
        }
    }
}
