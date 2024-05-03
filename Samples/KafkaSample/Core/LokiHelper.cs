using Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Core
{

    public class LokiHelper
    {
        private static readonly string lokiUrl = "http://localhost:3100/loki/api/v1/push";

        public static async Task<bool> SendLog(string data)
        {
            if (string.IsNullOrEmpty(data))
            {
                Console.WriteLine($"Data null yada boş olduğu için loki'ye gönderilmedi.");
                return false;
            }
            using HttpClient client = new HttpClient();

            var reqModel = new LokiRequestModel(data);
            string req = JsonSerializer.Serialize(reqModel);
            var content = new StringContent(req, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(lokiUrl, content);
            string responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            // İsteğin durumunu kontrol et
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--- Log verisi Loki'ye başarıyla gönderildi. ---");
                return true;
            }
            else
            {
                Console.WriteLine($"Loki'ye log verisi gönderilirken hata oluştu. Hata Kodu: {response.StatusCode}");
                return false;
            }

        }
    }
}
