using AttendEdgeMobileClient.Models;
using Newtonsoft.Json;

namespace AttendEdgeMobileClient.APIService
{
    public class ApiResponseHandler
    {
        public static async Task<Response> HandlerHttpResponse<T>(HttpResponseMessage responseMessage)
        {
            Response response = new Response();
            try
            {
                string jsonResult = responseMessage.Content != null ? await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false) : null;

                if (responseMessage.IsSuccessStatusCode &&
                    (responseMessage.StatusCode == System.Net.HttpStatusCode.OK ||
                    responseMessage.StatusCode == System.Net.HttpStatusCode.NoContent))
                {
                    response.IsSuccess = true;
                    response.StatusCode = System.Net.HttpStatusCode.OK;
                    response.Value = JsonConvert.DeserializeObject<T>(jsonResult);
                }
                else if (responseMessage.IsSuccessStatusCode && responseMessage.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.BadRequest;
                    response.ErrorMessage = JsonConvert.DeserializeObject<ApiMessage>(jsonResult)?.Message;
                }
                else if (!responseMessage.IsSuccessStatusCode && responseMessage.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    response.IsSuccess = false;
                    response.StatusCode = System.Net.HttpStatusCode.Unauthorized;
                    response.ErrorMessage = JsonConvert.DeserializeObject<ApiMessage>(jsonResult)?.Message;
                    await SessionManager.LogoutAsync();
                }
                else
                {
                    response.IsSuccess = false;
                    response.StatusCode = responseMessage.StatusCode;
                    response.ErrorMessage = jsonResult;
                }
            }
            catch (Exception ex)
            {
                _ = ex.Message;
            }
            return response;
        }
    }
}
