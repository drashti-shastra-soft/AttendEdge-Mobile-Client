using System.Net.Http.Headers;
using AttendEdgeMobileClient.Models;
using Newtonsoft.Json;


namespace AttendEdgeMobileClient.APIService
{
    public class ApiServiceHandler : IDisposable
    {
        HttpClient httpClient;
        public HttpClient CreateHttpClient(string authToken)
        {
            try
            {

                //#if IOS || MACCATALYST
                //                // For iOS & Mac Catalyst
                //                var handler = new NSUrlSessionHandler
                //                {
                //                    TrustOverrideForUrl = (sender, url, trust) =>
                //                    {
                //                        // url is an NSUrl (Apple native object)
                //                        if (url != null && url == "service.attendedge.in")
                //                            return true;

                //                        return false; // Reject all other hosts
                //                    }
                //                };
                //                httpClient = new HttpClient(handler);
                //#else
                // For Android & Windows
                var handler = new HttpClientHandler();
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
                {
                    if (message.RequestUri.Host == "service.attendedge.in")
                        return true;

                    return errors == System.Net.Security.SslPolicyErrors.None;
                };

                httpClient = new HttpClient(handler);
                //#endif

                httpClient.Timeout = TimeSpan.FromMinutes(15);
                httpClient.BaseAddress = new Uri(App.baseUrl);
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //authToken.IsNormalized() 
                if (!string.IsNullOrEmpty(authToken))
                {
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authToken);
                }

            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return httpClient;
        }

        public async Task<Response> GetAsync<T>(string uri, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);
                var responseMessage = await httpClient.GetAsync(uri).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<Response> PostAsync<T, TR>(string uri, T data, string authToken = "")
        {
            try
            {
                //string endPoint = $"{App.baseUrl}{uri}";
                HttpClient httpClient = CreateHttpClient(authToken);

                #region [ Ignore Null value in json ]
                JsonSerializerSettings settings = new JsonSerializerSettings();
                settings.NullValueHandling = NullValueHandling.Ignore;
                #endregion

                var jsonString = JsonConvert.SerializeObject(data, formatting: Formatting.Indented);
                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseMessage = await httpClient.PostAsync(uri, content).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<TR>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<Response> PostAsync<T>(string uri, T data, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);

                #region [ Ignore Null value in json ]
                //JsonSerializerSettings settings = new JsonSerializerSettings();
                //settings.NullValueHandling = NullValueHandling.Ignore;
                #endregion

                var jsonString = JsonConvert.SerializeObject(data, formatting: Formatting.Indented);
                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseMessage = await httpClient.PostAsync(uri, content).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<Response> PutAsync<T>(string uri, T data, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);

                #region [ Ignore Null value in json ]
                //JsonSerializerSettings settings = new JsonSerializerSettings();
                //settings.NullValueHandling = NullValueHandling.Ignore; 
                #endregion

                var jsonString = JsonConvert.SerializeObject(data, formatting: Formatting.Indented);
                var content = new StringContent(JsonConvert.SerializeObject(data));
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                var responseMessage = await httpClient.PutAsync(uri, content).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<Response> DeleteAsync<T>(string uri, string authToken = "")
        {
            try
            {
                HttpClient httpClient = CreateHttpClient(authToken);
                var responseMessage = await httpClient.DeleteAsync(uri).ConfigureAwait(false);
                return await ApiResponseHandler.HandlerHttpResponse<T>(responseMessage);
            }
            catch (Exception ex)
            {
                _ = ex.Message;
                return null;
            }
        }

        public async Task<string> GetBase64(string url)
        {
            string base64String = string.Empty;
            try
            {
                using (var client = new HttpClient())
                {
                    var response = await client.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await response.Content.ReadAsStreamAsync();
                            var bytes = memoryStream.ToArray();
                            base64String = Convert.ToBase64String(bytes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return base64String;
        }

        public void Dispose()
        {
            //httpClient = null;
            //GC.SuppressFinalize(this);
        }
    }
}
