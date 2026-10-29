using AttendEdgeMobileClient.Models;
using AttendEdgeMobileClient.Utility;

namespace AttendEdgeMobileClient.APIService
{
    public class AuthenticationAPI
    {
        Response requestResponse;

        public AuthenticationAPI()
        {
            requestResponse = new Response();
        }

        public async Task<Response> AuthenticateAsync(Credentials mAuthentication)
        {
            if (Common.CheckInternetConnection())
            {
                using (ApiServiceHandler apiServiceHandler = new ApiServiceHandler())
                {
                    requestResponse = await apiServiceHandler.PostAsync<Credentials, User>(EndPointsUrl.Authentication, mAuthentication);

                    if (requestResponse == null && !requestResponse.IsSuccess)
                    {
                        await App.Current.MainPage.DisplayAlert("Alert", $"{requestResponse.ErrorMessage}", "Ok");
                    }
                }
            }
            else
            {
                //Common.ErrorMessage(AppConstraints.NoInternet_Connection);
            }
            return requestResponse;
        }
    }
}
