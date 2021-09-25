using DiscordLib.Net.Payloads;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordLib.Net
{
    class RestClient
    {
        private HttpClient _httpClient;
        public HttpRequestHeaders Headers { get { return _httpClient.DefaultRequestHeaders; } }

        public RestClient(Uri baseUri, string userAgent)
        {
            var handler = new HttpClientHandler()
            {
                UseCookies = true,
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
            };

            _httpClient = new HttpClient(handler);
            _httpClient.BaseAddress = baseUri;

            //_httpClient.DefaultRequestHeaders.ConnectionClose = false;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", userAgent);
            //_httpClient.DefaultRequestHeaders.Add("Connection", "Keep-Alive");
            _httpClient.DefaultRequestHeaders.Add("Keep-Alive", "timeout=600");
        }

        public async Task<RestResponse<T>> SendRequestAsync<T>(RestRequest request)
        {
            await TaskEx.Yield();

            Debug.WriteLine(request.ToString());

            var httpRequestMessage = request.CreateRequestMessage();
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
            var content = await httpResponseMessage.Content.ReadAsStreamAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                var error = await DeserializeFromStreamAsync<Error>(content);
                throw new DiscordApiException(error.Code, error.Message);
            }

            var t = await DeserializeFromStreamAsync<T>(content);
            return new RestResponse<T>((int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, t);
        }

        public async Task<RestResponse<string>> SendRequestAsync(RestRequest request)
        {
            await TaskEx.Yield();

            Debug.WriteLine(request.ToString());

            var httpRequestMessage = request.CreateRequestMessage();
            var httpResponseMessage = await _httpClient.SendAsync(httpRequestMessage, HttpCompletionOption.ResponseHeadersRead);
            var content = await httpResponseMessage.Content.ReadAsStringAsync();

            if (!httpResponseMessage.IsSuccessStatusCode)
            {
                if (content.StartsWith("{")) // dirty check for JSON
                {
                    var error = JsonConvert.DeserializeObject<Error>(content);
                    throw new DiscordApiException(error.Code, error.Message);
                }

                httpResponseMessage.EnsureSuccessStatusCode();
            }

            return new RestResponse<string>((int)httpResponseMessage.StatusCode, httpResponseMessage.ReasonPhrase, content);
        }

        private static async Task<T> DeserializeFromStreamAsync<T>(Stream str)
        {
            using (var reader = new StreamReader(str))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var ser = new JsonSerializer();
                return await TaskEx.Run(() => ser.Deserialize<T>(jsonReader));
            }
        }
    }
}
