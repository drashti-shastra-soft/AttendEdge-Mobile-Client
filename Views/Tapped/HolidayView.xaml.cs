using AttendEdgeMobileClient.APIService;
using AttendEdgeMobileClient.Model;
using AttendEdgeMobileClient.Models;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using Java.Net;
using System.Diagnostics;
using System.Text.Json;
using System.Threading.Tasks;

namespace AttendEdgeMobileClient.Views.Tapped;

public partial class HolidayView : ContentView
{
    private List<Holiday> _allHolidays = new();
    public HolidayLister mHolidayListers;
    public HolidayView()
    {
        InitializeComponent();
        mHolidayListers = new HolidayLister();
        Loaded += HolidayView_Loaded;

    }

    private async void HolidayView_Loaded(object? sender, EventArgs e)
    {
        await GetHolidays();
    }

    async Task GetHolidays()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;
            lstHoliday.ItemsSource = null;
            await PopLoaderService.ShowAsync();
            var mResponse = await new ApiServiceHandler().GetAsync<List<Holiday>>(EndPointsUrl.GetHoliday, Common.AuthToken);

            Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(mResponse.Value, new JsonSerializerOptions
            {
                WriteIndented = true
            }));

            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value != null &&
                    mResponse.Value is List<Holiday> holiday &&
                    holiday.Count > 0)
                {
                    _allHolidays = holiday;
                    lstHoliday.ItemsSource = _allHolidays;

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

    private async void OnRefreshing(object sender, EventArgs e)
    {
        HolidayRefreshView.IsRefreshing = true;
        await GetHolidays();
        HolidayRefreshView.IsRefreshing = false;
    }
}


//var mResponse = await new ApiServiceHandler()
// .PostAsync<HolidayLister>(
//     EndPointsUrl.GetHoliday,
//     mHolidayListers,
//     Common.AuthToken);


//if (mResponse != null && mResponse.IsSuccess)
//{
//    if (mResponse.Value != null &&
//        mResponse.Value is HolidayLister holidayListers &&
//        holidayListers.List != null &&
//        holidayListers.List.Count > 0)
//    {
//        _allHolidays = holidayListers.List.ToList();
//        //_allHolidays = holidayListers.List.OrderByDescending(x => x.Date).ToList();
//        lstHoliday.ItemsSource = _allHolidays;
//    }

//}