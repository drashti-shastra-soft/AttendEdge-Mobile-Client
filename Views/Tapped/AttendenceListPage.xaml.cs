using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using AttendEdgeMobileClient.APIService;
using AttendEdgeMobileClient.Models;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using Syncfusion.Maui.Data;

namespace AttendEdgeMobileClient.Views.Tapped;

public partial class AttendenceListPage : ContentPage
{
    AttendanceLister mAttendanceLister;
    public AttendenceListPage()
    {
        InitializeComponent();
        mAttendanceLister = new AttendanceLister();
    }

    protected async override void OnAppearing()
    {
        base.OnAppearing();
        await GetAtendance();
    }

    async Task GetAtendance()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            //lblRecoedNotFond.IsVisible = false;
            lstAttendence.ItemsSource = null;
            await PopLoaderService.ShowAsync();
            var mResponse = await new ApiServiceHandler().PostAsync<AttendanceLister>(uri: EndPointsUrl.GetAttendence, mAttendanceLister, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess && mResponse.Value is AttendanceLister attendanceLister && attendanceLister != null && attendanceLister.List.Count > 0)
            {
                lstAttendence.ItemsSource = attendanceLister.List.ToList();
            }
            else
            {
                //lblRecoedNotFond.IsVisible = true;
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

    private async void BtnBack_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        await Navigation.PopAsync();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        AttendenceListRefreshView.IsRefreshing = true;
        await GetAtendance();
        AttendenceListRefreshView.IsRefreshing = false;
    }
}