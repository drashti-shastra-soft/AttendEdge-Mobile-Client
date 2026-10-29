using AttendEdgeMobileClient.APIService;
using AttendEdgeMobileClient.Model;
using AttendEdgeMobileClient.PopupPages;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using CommunityToolkit.Maui.Views;
using Mopups.Animations;
using Mopups.Enums;
using Mopups.Services;
using System.Diagnostics;
using System.Text.Json;
using static AttendEdgeMobileClient.Utility.AppEnum;

namespace AttendEdgeMobileClient.Views.Tapped;

public partial class LeaveBalanceView : ContentView
{
    private List<LeaveRequest> _allLeaveRequest = new();
    LeaveRequestLister mLeaveRequestLister;
    bool isFilterOpen = false;
    private bool _isApproved = false;
    private bool _isOneDay = false;
    private bool _isTwoDay = false;
    private bool _isThreeDayOrMore = false;
    private bool _isCasualLeave = false;
    private bool _isSickLeave = false;
    private bool _isPaisLeave = false;
    private bool _isHalf = false;
    public LeaveBalanceView()
    {
        InitializeComponent();
        mLeaveRequestLister = new LeaveRequestLister();
        Loaded += LeaveBalanceView_Loaded;
    }

    private async void LeaveBalanceView_Loaded(object? sender, EventArgs e)
    {
        await GetLeaveRequest();
    }

    private async Task GetLeaveRequest()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            clsLeaveRequest.ItemsSource = null;
            await PopLoaderService.ShowAsync();

            //var mResponse = await new ApiServiceHandler().GetAsync<List<LeaveRequest>>(EndPointsUrl.GetLeave, Common.AuthToken);

            //Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(mResponse.Value, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //}));

            //if (mResponse != null && mResponse.IsSuccess)
            //{
            //    if (mResponse.Value != null &&
            //        mResponse.Value is List<LeaveRequest> mLeaveRequest &&
            //        mLeaveRequest.Count > 0)
            //    {
            //        foreach (var item in mLeaveRequest)
            //        {
            //            //if (item.Status == LeaveStatus.Approved.ToString())
            //            //    item.LeaveStatus = 3;
            //            //else if (item.Status == LeaveStatus.Pending.ToString())
            //            //    item.LeaveStatus = 4;
            //            //else if (item.Status == LeaveStatus.Reject.ToString())
            //            //    item.LeaveStatus = 5;

            //            if (item.IsHalfDay)
            //                item.LeaveRequestType = "Half Day";
            //            else
            //                item.LeaveRequestType = "Full Day";

            //            //item.TotalDays = (item.EndDate - item.StartDate).Days + 1;
            //        }
            //        _allLeaveRequest = mLeaveRequest;
            //        clsLeaveRequest.ItemsSource = _allLeaveRequest;
            //        //lstHoliday.ItemsSource = _allHolidays;

            //    }
            //}

            var mResponce = await new ApiServiceHandler().PostAsync<LeaveRequestLister>(EndPointsUrl.GetAllLeave, mLeaveRequestLister, Common.AuthToken);

            //Debug.WriteLine(System.Text.Json.JsonSerializer.Serialize(mResponce.Value, new JsonSerializerOptions
            //{
            //    WriteIndented = true
            //}));

            if (mResponce != null && mResponce.Value != null)
            {
                if (mResponce.Value is LeaveRequestLister mLeaveRequest && mLeaveRequest != null)
                {
                    foreach (var item in mLeaveRequest.List)
                    {
                        if (item.IsHalfDay)
                            item.LeaveRequestType = "Half Day";
                        else
                            item.LeaveRequestType = "Full Day";
                    }
                    clsLeaveRequest.ItemsSource = mLeaveRequest.List.ToList();

                    int sickLeaveCount = mLeaveRequest.List.Count(x =>
                        string.Equals(x.LeaveType, "Sick",
                            StringComparison.OrdinalIgnoreCase));

                    int casualLeaveCount = mLeaveRequest.List.Count(x =>
                        string.Equals(x.LeaveType, "Casual",
                            StringComparison.OrdinalIgnoreCase));

                    int paidLeaveCount = mLeaveRequest.List.Count(x =>
                        string.Equals(x.LeaveType, "Paid",
                            StringComparison.OrdinalIgnoreCase));

                    // Display counts
                    lblSickLeaveCount.Text = sickLeaveCount.ToString("00");
                    lblCasualLeaveCount.Text = casualLeaveCount.ToString("00");
                    lblPaidLeaveCount.Text = paidLeaveCount.ToString("00");
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


    private async void LblFilter_Tapped(object sender, TappedEventArgs e)
    {
        var popup = new FilterPopup();
        popup.Animation = new MoveAnimation
        {
            PositionIn = MoveAnimationOptions.Bottom,
            PositionOut = MoveAnimationOptions.Bottom,
            DurationIn = 300,
            DurationOut = 300
        };

        popup.FilterApplied += FilterPopup_FilterApplied;

        await MopupService.Instance.PushAsync(popup);
    }

    private async void FilterPopup_FilterApplied1(object? sender, FilterResult result)
    {
        this.mLeaveRequestLister.SearchCriteria = new LeaveRequest();

        mLeaveRequestLister.SearchCriteria.Status = result.IsApproved ? "Approved" : "Rejected";
        mLeaveRequestLister.SearchCriteria.LeaveType = result.IsCasualLeave ? "Casual" : result.IsSickLeave ? "Sick" : result.IsPaidLeave ? "Paid" : "Unknown";
        mLeaveRequestLister.SearchCriteria.IsHalfDay = result.IsHalfDay ? true : false;
        mLeaveRequestLister.SearchCriteria.StartDate = result.StartDate;
        mLeaveRequestLister.SearchCriteria.EndDate = result.EndDate;

        await GetLeaveRequest();


        //// Status
        //if (result.IsApproved)
        //{
        //    mLeaveRequestLister.SearchCriteria.Status = "Approve";
        //}
        //else
        //{
        //    mLeaveRequestLister.SearchCriteria.Status = "Reject";
        //}

        //// Leave Type
        //if (result.IsCasualLeave)
        //{
        //    mLeaveRequestLister.SearchCriteria.LeaveType = "Causal";
        //}
        //else if (result.IsSickLeave)
        //{
        //    mLeaveRequestLister.SearchCriteria.LeaveType = "Sick";
        //}
        //else if (result.IsPaidLeave)
        //{
        //    mLeaveRequestLister.SearchCriteria.LeaveType = "Paid";
        //}
        //else
        //{
        //    mLeaveRequestLister.SearchCriteria.LeaveType = "Unknown";
        //}

        //// Half / Full
        //mLeaveRequestLister.SearchCriteria.IsHalfDay =
        //    result.IsHalfDay;
        //mLeaveRequestLister.SearchCriteria.StartDate =
        //    result.StartDate;
        //mLeaveRequestLister.SearchCriteria.EndDate =
        //    result.EndDate;

        //await GetLeaveRequest();
    }

    private async void FilterPopup_FilterApplied(
    object? sender,
    FilterResult result)
    {
        mLeaveRequestLister.SearchCriteria = new LeaveRequest();

        // Status
        if (result.IsApproved)
        {
            mLeaveRequestLister.SearchCriteria.Status = "Approved";
        }
        else if (result.IsRejected)
        {
            mLeaveRequestLister.SearchCriteria.Status = "Rejected";
        }
        else
        {
            mLeaveRequestLister.SearchCriteria.Status = null;
        }

        // Leave Type
        if (result.IsCasualLeave)
        {
            mLeaveRequestLister.SearchCriteria.LeaveType = "Casual";
        }
        else if (result.IsSickLeave)
        {
            mLeaveRequestLister.SearchCriteria.LeaveType = "Sick";
        }
        else if (result.IsPaidLeave)
        {
            mLeaveRequestLister.SearchCriteria.LeaveType = "Paid";
        }
        else
        {
            mLeaveRequestLister.SearchCriteria.LeaveType = "Unknown";
        }

        // Half / Full Day
        mLeaveRequestLister.SearchCriteria.IsHalfDay =
            result.IsHalfDay;

        // Date
        mLeaveRequestLister.SearchCriteria.StartDate =
            result.StartDate;

        mLeaveRequestLister.SearchCriteria.EndDate =
            result.EndDate;

        await GetLeaveRequest();
    }

    private async void OnRefreshing(object sender, EventArgs e)
    {
        LeaveBalanceRefreshView.IsRefreshing = true;
        mLeaveRequestLister.SearchCriteria = new LeaveRequest();
        await GetLeaveRequest();
        LeaveBalanceRefreshView.IsRefreshing = false;
    }
}