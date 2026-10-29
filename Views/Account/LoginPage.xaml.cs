using Acr.UserDialogs;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using AttendEdgeMobileClient.Views.Chart;
using AttendEdgeMobileClient.Views.Tapped;
using Mopups.Services;
using System.Net.Mail;
using System.Text.RegularExpressions;

namespace AttendEdgeMobileClient.Views.Account;

public partial class LoginPage : ContentPage
{
    public bool isRememberMe = false;

    public LoginPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        isRememberMe = AppSetting.GetRememberMe();
        if (isRememberMe)
        {
            TxtEmailAddress.Text = AppSetting.GetEmail();
            TxtPassword.Text = AppSetting.GetPassword();
            imgCheckBox.Source = AppConstraints.Check;
        }
        else
        {
            TxtEmailAddress.Text = string.Empty;
            TxtPassword.Text = string.Empty;
            imgCheckBox.Source = AppConstraints.UnCheck;
        }
    }

    private bool ValidateControl()
    {
        bool isValid = false;
        if (string.IsNullOrWhiteSpace(TxtEmailAddress.Text) || string.IsNullOrWhiteSpace(TxtPassword.Text))
        {
            Common.ErrorMessage("Please enter username and password!");
        }
        else if (string.IsNullOrWhiteSpace(TxtEmailAddress.Text))
        {
            Common.ErrorMessage("Please enter email address!");
        }
        else if (string.IsNullOrWhiteSpace(TxtPassword.Text))
        {
            Common.ErrorMessage("Please enter password!");
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

    //private bool IsValidEmail(string email)
    //{
    //    try
    //    {
    //        var mailAddress = new MailAddress(email);
    //        return mailAddress.Address == email;
    //    }
    //    catch
    //    {
    //        return false;
    //    }
    //}


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
    private bool IsValidEmail1(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        return Regex.IsMatch(
            email,
            @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
            RegexOptions.IgnoreCase);
    }

//    protected override bool OnBackButtonPressed()
//    {
//        Dispatcher.Dispatch(async () =>
//        {
//            var popup = new CommonPopupPage(isExit: true);

//            popup.logoutEvent += async (s, e) =>
//            {
//                await MopupService.Instance.PopAsync();

//#if ANDROID
//                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
//#endif
//            };

//            await MopupService.Instance.PushAsync(popup);
//        });

//        return true;
//    }

    protected override bool OnBackButtonPressed()
    {
        // Show the confirmation dialog
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            bool exit = await DisplayAlert("Exit App",
                "Are you sure you want to exit?", "Yes", "No");

            if (exit)
            {
#if ANDROID
                // Close the application
                Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#endif
            }
        });

        // Return true to cancel the default back navigation
        return true;
    }
    private void TxtEmailAddress_Focused(object sender, FocusEventArgs e)
    {
        //lblEmail.IsVisible = true;
    }

    private void TxtPassword_Focused(object sender, FocusEventArgs e)
    {
        //lblPassword.IsVisible = true;
    }


    private async void BtnLogin_Clicked(object sender, EventArgs e)
    {
        try
        {

            if (!await NetworkService.CheckInternetAsync())
                return;

            await Common.ControlBounceEffect(sender);
            //await Navigation.PushAsync(new HomeTabbedPage());
            await ProcessToLogin();
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"LoginPage/BtnLogin_Clicked: {ex.Message}");
        }
    }

    private async Task ProcessToLogin()
    {
        try
        {
            if (!ValidateControl())
                return;

            var mCredential = new Models.Credentials
            {
                EmailAddress = TxtEmailAddress.Text?.Trim(),
                Password = TxtPassword.Text?.Trim()
            };
            //Models.Credentials mCredential = new Models.Credentials();
            //mCredential.EmailAddress = TxtEmailAddress.Text;
            //mCredential.Password = TxtPassword.Text;

            if (mCredential != null)
            {
                await PopLoaderService.ShowAsync();
                var mResponce = await new APIService.ApiServiceHandler().PostAsync<Models.Credentials, Models.User>(EndPointsUrl.Authentication, mCredential);
                if (mResponce == null || !mResponce.IsSuccess)
                {
                    Common.ErrorMessage(mResponce.ErrorMessage);
                    return;
                }

                if (mResponce != null && mResponce.IsSuccess)
                {
                    if (mResponce.Value is Models.User mUser)
                    {
                        Common.mUser = mUser;
                        if (!string.IsNullOrEmpty(Common.mUser.Token))
                            Common.AuthToken = Common.mUser.Token;

                        if (isRememberMe)
                        {
                            AppSetting.SetEmail(TxtEmailAddress.Text);
                            AppSetting.SetPassword(TxtPassword.Text);
                        }
                        else
                        {
                            AppSetting.SetEmail(string.Empty);
                            AppSetting.SetPassword(string.Empty);
                        }

                        AppSetting.SetRememberMe(isRememberMe);

                        await Navigation.PushAsync(new CustomeTappedPage());
                    }
                }
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

    private async void BtnLogin_Clicked1(object sender, EventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        //await Navigation.PushAsync(new Dashboard.DashboardPage());
        // await Navigation.PushAsync(new CustomeTappedPage());
        //await Navigation.PushAsync(new ChangePasswordPage());
        if (!ValidateControl())
            return;

        Models.Credentials mCredential = new Models.Credentials();
        mCredential.EmailAddress = TxtEmailAddress.Text;
        mCredential.Password = TxtPassword.Text;

        if (mCredential != null)
        {
            await PopLoaderService.ShowAsync();
            var mResponce = await new APIService.ApiServiceHandler().PostAsync<Models.Credentials, Models.User>(EndPointsUrl.Authentication, mCredential);
            if (mResponce == null || !mResponce.IsSuccess)
            {
                Common.ErrorMessage(mResponce.ErrorMessage);
                return;
            }

            if (mResponce != null && mResponce.IsSuccess)
            {
                if (mResponce.Value is Models.User mUser)
                {
                    Common.mUser = mUser;
                    if (!string.IsNullOrEmpty(Common.mUser.Token))
                        Common.AuthToken = Common.mUser.Token;

                    if (isRememberMe)
                    {
                        AppSetting.SetEmail(TxtEmailAddress.Text);
                        AppSetting.SetPassword(TxtPassword.Text);
                    }
                    else
                    {
                        AppSetting.SetEmail(string.Empty);
                        AppSetting.SetPassword(string.Empty);
                    }

                    AppSetting.SetRememberMe(isRememberMe);

                    await Navigation.PushAsync(new CustomeTappedPage());
                }
            }
            await PopLoaderService.HideAsync();
        }
    }

    private void stkRememberMe_Tapped(object sender, TappedEventArgs e)
    {
        bool isChecked = imgCheckBox.Source?.ToString().Contains("icon_uncheck.png") == true;
        imgCheckBox.Source = isChecked ? "icon_check.png" : "icon_uncheck.png";
        isRememberMe = isChecked;
    }

    private async void lblForgotPassword_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        await Navigation.PushAsync(new ForgotPasswordPage());
        //await Navigation.PushAsync(new TakeSelfiePage());
        //await Navigation.PushAsync(new HomePage());
    }

    private void ImgEye_Tapped1(object sender, TappedEventArgs e)
    {
        bool isHidden = ImgEye.Source?.ToString().Contains("icon_unhide.png") == true;
        ImgEye.Source = isHidden ? "icon_hide.png" : "icon_unhide.png";
        TxtPassword.IsPassword = isHidden;
    }

    private void ImgEye_Tapped(object sender, TappedEventArgs e)
    {
        bool isHidden = ImgEye.Source?.ToString().Contains("icon_unhide.png") == true;
        ImgEye.Source = isHidden ? "icon_hide.png" : "icon_unhide.png";
        TxtPassword.IsPassword = isHidden;
    }
}