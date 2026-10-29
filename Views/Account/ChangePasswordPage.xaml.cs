using AttendEdgeMobileClient.Model;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using System.Threading.Tasks;

namespace AttendEdgeMobileClient.Views.Account;

public partial class ChangePasswordPage : ContentPage
{
    public ChangePasswordPage()
    {
        InitializeComponent();
    }


    private void ImgCurrentPasswordEye_Tapped(object sender, TappedEventArgs e)
    {
        bool isHidden = ImgCurrentPasswordEye.Source?.ToString().Contains("icon_unhide.png") == true;
        ImgCurrentPasswordEye.Source = isHidden ? "icon_hide.png" : "icon_unhide.png";
        txtCurrentPassword.IsPassword = isHidden;
    }

    private void ImgNewPasswordEye_Tapped(object sender, TappedEventArgs e)
    {
        bool isHidden = ImgNewPasswordEye.Source?.ToString().Contains("icon_unhide.png") == true;
        ImgNewPasswordEye.Source = isHidden ? "icon_hide.png" : "icon_unhide.png";
        TxtNewPassword.IsPassword = isHidden;
    }

    private void ImgConfirmPasswordEye_Tapped(object sender, TappedEventArgs e)
    {
        bool isHidden = ImgConfirmEye.Source?.ToString().Contains("icon_unhide.png") == true;
        ImgConfirmEye.Source = isHidden ? "icon_hide.png" : "icon_unhide.png";
        TxtConfirmPassword.IsPassword = isHidden;
    }

    private bool ValidateControl()
    {
        bool isValid = false;
        if (string.IsNullOrWhiteSpace(TxtNewPassword.Text) || string.IsNullOrWhiteSpace(TxtConfirmPassword.Text))
        {
            Common.ErrorMessage("Please enter new password and confirm password!");
        }
        else if (string.IsNullOrWhiteSpace(TxtNewPassword.Text))
        {
            Common.ErrorMessage("Please enter new password!");
        }
        else if (string.IsNullOrWhiteSpace(TxtConfirmPassword.Text))
        {
            Common.ErrorMessage("Please enter confirm password!");
        }
        else if (TxtNewPassword.Text != TxtConfirmPassword.Text)
        {
            Common.ErrorMessage("Not match new password and confirm password!");
        }
        else
        {
            isValid = true;
        }

        return isValid;
    }

    private async void BtnUpdate_Clicked(object sender, EventArgs e)
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            await Common.ControlBounceEffect(sender);
            await ProcessToChangePassword();
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"ForgotPasswordPage: {ex.Message}");
        }
    }

    private async Task ProcessToChangePassword()
    {
        try
        {
            if (!ValidateControl())
                return;


            string currentPassword = txtCurrentPassword.Text?.Trim() ?? "";
            string newPassword = TxtNewPassword.Text?.Trim() ?? "";
            string confirmPassword = TxtConfirmPassword.Text?.Trim() ?? "";

            ChangePassword mChangePassword = new ChangePassword
            {
                UserId = Common.mUser.Id,
                CurrentPassword = currentPassword,
                NewPassword = newPassword,
                ConfirmPassword = confirmPassword

            };
            await PopLoaderService.ShowAsync();
            var mResponse = await new APIService.ApiServiceHandler().PutAsync<ChangePassword>(EndPointsUrl.ChangePassword, mChangePassword, Common.AuthToken);
            if (mResponse == null || !mResponse.IsSuccess)
            {
                Common.ErrorMessage(mResponse.ErrorMessage);
                return;
            }

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Password has been chnaged Successfully");
                ClearControl();
                await Navigation.PopAsync();
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
    private void ClearControl()
    {
        txtCurrentPassword.Text = string.Empty;
        TxtNewPassword.Text = string.Empty;
        TxtConfirmPassword.Text = string.Empty;
    }
    private async void ImgBack_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        await Navigation.PopAsync();
    }

}