using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Utils
{
    public static class HttpJson
    {
        #region Requests

        internal static HttpWebRequest CreateGetRequest(string Url)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(Url);
            webRequest.Method = WebRequestMethods.Http.Get;
            webRequest.Accept = "application/json";
            return webRequest;
        }

        internal static HttpWebRequest CreatePutRequest(string Url)
        {
            var webRequest = (HttpWebRequest) WebRequest.Create(Url);
            webRequest.Method = WebRequestMethods.Http.Put;
            webRequest.ContentType = "application/json";
            return webRequest;
        }

        #endregion

        #region Get

        #region GetRawJson

        public static async Task<string> GetRawJson(string Url)
        {
            var httpWebRequest = CreateGetRequest(Url);

            return new StreamReader((await httpWebRequest.GetResponseAsync()).GetResponseStream()).ReadToEnd();
        }

        public static async Task<string> GetRawJson(string Url, string Proxy, int Port)
        {
            var httpWebRequest = CreateGetRequest(Url);

            httpWebRequest.Proxy = new WebProxy(Proxy, Port);

            return new StreamReader((await httpWebRequest.GetResponseAsync()).GetResponseStream()).ReadToEnd();
        }

        public static async Task<string> GetRawJson(string Url, string Proxy, int Port, NetworkCredential Credentials)
        {
            var httpWebRequest = CreateGetRequest(Url);

            httpWebRequest.Proxy = new WebProxy(Proxy, Port) {Credentials = Credentials};

            return new StreamReader((await httpWebRequest.GetResponseAsync()).GetResponseStream()).ReadToEnd();
        }

        #endregion

        #region GetDeserialized

        public static async Task<T> GetDeserialized<T>(string Url)
            => JsonConvert.DeserializeObject<T>(await GetRawJson(Url));

        public static async Task<T> GetDeserialized<T>(string Url, string Proxy, int Port)
            => JsonConvert.DeserializeObject<T>(await GetRawJson(Url, Proxy, Port));


        public static async Task<T> GetDeserialized<T>(string Url, string Proxy, int Port,
            NetworkCredential Credentials)
            => JsonConvert.DeserializeObject<T>(await GetRawJson(Url, Proxy, Port, Credentials));

        #endregion

        #endregion

        #region Put

        #region PutRawJson

        public static async void PutRawJson(string Json, string Url)
        {
            var httpWebRequest = CreatePutRequest(Url);

            var requestStream = httpWebRequest.GetRequestStream();
            await requestStream.WriteAsync(Encoding.ASCII.GetBytes(Json), 0, Json.Length);
            requestStream.Close();
            await httpWebRequest.GetResponseAsync();
        }

        public static async void PutRawJson(string Json, string Url, string Proxy, int Port)
        {
            var httpWebRequest = CreatePutRequest(Url);

            httpWebRequest.Proxy = new WebProxy(Proxy, Port);

            var requestStream = httpWebRequest.GetRequestStream();
            await requestStream.WriteAsync(Encoding.ASCII.GetBytes(Json), 0, Json.Length);
            requestStream.Close();
            await httpWebRequest.GetResponseAsync();
        }

        public static async void PutRawJson(string Json, string Url, string Proxy, int Port,
            NetworkCredential Credentials)
        {
            var httpWebRequest = CreatePutRequest(Url);

            httpWebRequest.Proxy = new WebProxy(Proxy, Port) {Credentials = Credentials};

            using (var requestStream = httpWebRequest.GetRequestStream())
            {
                await requestStream.WriteAsync(Encoding.ASCII.GetBytes(Json), 0, Json.Length);
            }

            await httpWebRequest.GetResponseAsync();
        }

        #endregion

        #region PutObject

        public static async void PutObject(object @Object, string Url)
            => await Task.Run(() => PutRawJson(JsonConvert.SerializeObject(@Object, Formatting.Indented), Url));

        public static async void PutObject(object @Object, string Url, string Proxy, int Port)
            => await Task.Run(() =>
                PutRawJson(JsonConvert.SerializeObject(@Object, Formatting.Indented), Url, Proxy, Port));

        public static async void PutObject(object @Object, string Url, string Proxy, int Port,
            NetworkCredential Credentials)
            => await Task.Run(() => PutRawJson(JsonConvert.SerializeObject(@Object, Formatting.Indented), Url, Proxy,
                Port, Credentials));

        #endregion

        #endregion
    }
}
