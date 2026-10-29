using AttendEdgeMobileClient.Model;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using System.Net.Mail;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace AttendEdgeMobileClient.Views.Account;

public partial class ForgotPasswordPage : ContentPage
{
    public ForgotPasswordPage()
    {
        InitializeComponent();
    }

    private async void ImgBack_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        await Navigation.PopAsync();
    }

    private bool ValidateControl()
    {
        bool isValid = false;
        if (string.IsNullOrWhiteSpace(TxtEmailAddress.Text))
        {
            Common.ErrorMessage("Please enter email address!");
        }
        else if (!IsValidEmail(TxtEmailAddress.Text.Trim()))
        {
            Common.ErrorMessage("Please enter a valid email address!");
        }
        else
        {
            isValid = true;
        }

        return isValid;
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            var mailAddress = new MailAddress(email);

            return mailAddress.Address == email &&
                   mailAddress.Host.Contains('.') &&
                   !mailAddress.Host.EndsWith('.');
        }
        catch
        {
            return false;
        }
    }

    private async void BtnContinue_Clicked1(object sender, EventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        //await Navigation.PushAsync(new CodeVerificationPage());
        ForgotPassword mForgotPassword = new ForgotPassword();
        mForgotPassword.EmailAddress = TxtEmailAddress.Text;
        var mResponse = await new APIService.ApiServiceHandler().PostAsync<ForgotPassword>(EndPointsUrl.ForgotPassword, mForgotPassword);
    }

    private async void BtnContinue_Clicked(object sender, EventArgs e)
    {
        try
        {

            if (!await NetworkService.CheckInternetAsync())
                return;

            await Common.ControlBounceEffect(sender);
            await ProcessToForgotPassword();
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"ForgotPasswordPage: {ex.Message}");
        }
    }

    private async Task ProcessToForgotPassword()
    {
        try
        {
            if (!ValidateControl())
                return;

            ForgotPassword mForgotPassword = new ForgotPassword
            {
                EmailAddress = TxtEmailAddress.Text
            };
            await PopLoaderService.ShowAsync();
            var mResponse = await new APIService.ApiServiceHandler().PostAsync<ForgotPassword>(EndPointsUrl.ForgotPassword, mForgotPassword);
            if (mResponse == null || !mResponse.IsSuccess)
            {
                Common.ErrorMessage(mResponse.ErrorMessage);
                return;
            }

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Success");
                await Navigation.PopAsync();
                //Navigation.PushAsync(new OTPVerificationPage(txtEmail.Text));

            }


        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }


}
