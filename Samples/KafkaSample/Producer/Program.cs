using Confluent.Kafka;
using Core;

namespace Producer
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            int count = 0;
            while (true)
            {
                Console.WriteLine("Mesaj göndermek için bir topic numarası girin veya 'q' tuşuna basarak programı sonlandırın. \n 1- sample-one \n 2- sample-two \n q- Çıkış");
                var topic = Console.ReadLine();
                if (topic?.ToString() == "q")
                {
                    break;
                }
                topic = topic == "1" ? "sample-one" : "sample-two";

                Console.WriteLine("Mesajınızı Giriniz: ");
                var input = Console.ReadLine();

                var sendMsg = string.Format("count:{0},msg:{1}", (++count).ToString(), input?.ToString());

                DeliveryResult<Null, string> deliveryReport = await ProducerHelper.SendKafkaData("localhost:9092", topic, sendMsg);
                var report = string.Format("\n Gönderim başarılı. Msg:{0}, DeliveryReport.TopicPartitionOffset:{1}", sendMsg, deliveryReport.TopicPartitionOffset);

                Console.WriteLine("\n" + report);
                await Task.Delay(500);
            }
        }
    }
}
