using System.Threading.Tasks;
using AttendEdgeMobileClient.CustomeControls;
using AttendEdgeMobileClient.Utility;
using static System.Net.Mime.MediaTypeNames;

namespace AttendEdgeMobileClient.Views.Account;

public partial class CodeVerificationPage : ContentPage
{
    public CodeVerificationPage()
    {
        InitializeComponent();
    }

    private async void ImgBack_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        await Navigation.PopAsync();
    }

    private async void BtnVerify_Clicked(object sender, EventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        await Navigation.PushAsync(new LoginPage());
    }

    private async void lblResend_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
    }


    private void EntVerificationCode_Tapped(object sender, TextChangedEventArgs e)
    {
        if (sender is BorderlessEntry entry)
        {
            if (entry.ClassId == "1")
            {
                Second.Focus();
            }
            if (entry.ClassId == "2")
            {
                Third.Focus();
            }
            if (entry.ClassId == "3")
            {
                Fourth.Focus();
            }
        }
    }
}