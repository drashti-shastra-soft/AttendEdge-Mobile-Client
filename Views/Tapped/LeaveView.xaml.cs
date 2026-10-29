using System.Collections.ObjectModel;
using System.ComponentModel;
using Android.Views.Animations;
using AttendEdgeMobileClient.Interface;
using AttendEdgeMobileClient.Model;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using Kotlin;

namespace AttendEdgeMobileClient.Views.Tapped;

public partial class LeaveView : ContentView, IRefreshable, INotifyPropertyChanged
{
    public event EventHandler refreshEvent;
    bool isHalfDaySelected = false;
    bool isFirstHalf = false;
    public ObservableCollection<LeaveTypeModel> LeaveTypes { get; set; }
    private string _selectedLeaveType;
    public string SelectedLeaveType
    {
        get => _selectedLeaveType;
        set
        {
            _selectedLeaveType = value;
            OnPropertyChanged(nameof(SelectedLeaveType));
        }
    }
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged(string name) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    public LeaveView()
    {
        InitializeComponent();
        LeaveTypes = new ObservableCollection<LeaveTypeModel>
        {
            new LeaveTypeModel { Id = 1, Name = "Causal" },
            new LeaveTypeModel { Id = 2, Name = "Paid" },
            new LeaveTypeModel { Id = 3, Name = "Sick" }
        };

        this.BindingContext = this;
    }
    public async Task RefreshDataAsync()
    {
        await SaveLeaveRequest();
    }
    //LeaveRequest PrepareLeaveRequestObject()
    //{
    //    LeaveRequest mLeaveRequest = new LeaveRequest();
    //    if (mLeaveRequest != null && mLeaveRequest.Id == 0)
    //    {
    //        //mLeaveRequest.CreatedBy = Common.mUser.Id;
    //        //mLeaveRequest.CreatedDate = DateTime.Now;
    //    }

    //    mLeaveRequest.UserId = Common.mUser.Id;
    //    mLeaveRequest.CompanyId = Common.mUser.CompanyId;
    //    //mLeaveRequest.Username = $"{Common.mUser.FirstName} {Common.mUser.FirstName}";
    //    mLeaveRequest.StartDate = dtpckStart.Date;
    //    mLeaveRequest.EndDate = dtpckEnd.Date;
    //    mLeaveRequest.Reason = txtLeaveReason.Text;
    //    mLeaveRequest.LastModifiedDate = DateTime.Now.Date;
    //    mLeaveRequest.LastModifiedBy = Common.mUser.Id;

    //    if (isHalfDaySelected)
    //        mLeaveRequest.HalfType = isFirstHalf ? "First Half" : "Second Half";

    //    mLeaveRequest.IsHalfDay = isHalfDaySelected;


    //    return mLeaveRequest;
    //}

    private LeaveRequest PrepareLeaveRequestObject()
    {
        var mLeaveRequest = new LeaveRequest
        {
            UserId = Common.mUser.Id,
            CompanyId = Common.mUser.CompanyId,
            StartDate = dtpckStart.Date,
            EndDate = dtpckEnd.Date,
            Reason = txtLeaveReason.Text,
            LastModifiedBy = Common.mUser.Id,

            IsHalfDay = isHalfDaySelected,
            HalfType = isHalfDaySelected
                ? (isFirstHalf ? "First Half" : "Second Half")
                : null
        };

        return mLeaveRequest;
    }

    private async void BtnApplyLeave_Clicked(object sender, EventArgs e)
    {
        await SaveLeaveRequest();
    }
    async Task SaveLeaveRequest()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            await PopLoaderService.ShowAsync();
            var mLeaveRequest = PrepareLeaveRequestObject();
            if (mLeaveRequest != null)
            {
                var mResponse = await new APIService.ApiServiceHandler().PostAsync<LeaveRequest>(EndPointsUrl.AddLeave, mLeaveRequest, Common.AuthToken);
                if (mResponse != null && mResponse.IsSuccess)
                {
                    if (mResponse.Value is LeaveRequest efLeaveRequest)
                    {
                        //mLeaveRequest = new LeaveRequest();
                        // Reset form after successful API call
                        ResetLeaveForm();

                        // Optional success message
                         Common.SuccessMessage("Leave applied successfully.");

                    }

                }
                else
                {
                    Common.ErrorMessage("Opps! something went wrong");
                }
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage(ex.ToString());
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }

    }

    private void ResetLeaveForm()
    {
        // Reset dates
        dtpckStart.Date = DateTime.Today;
        dtpckEnd.Date = DateTime.Today;

        // Reset reason
        txtLeaveReason.Text = string.Empty;

        // Reset Full Day / Half Day
        btnFullday.BackgroundColor = (Color)Application.Current.Resources["Primary"];
        btnFullday.TextColor = Colors.White;

        btnHalfDay.BackgroundColor = Colors.Transparent;
        btnHalfDay.TextColor = (Color)Application.Current.Resources["PrimaryBlack"];

        // Hide First Half / Second Half
        bdrSubLeaveType.IsVisible = false;

        // Reset sub leave type
        btnFirstHalf.BackgroundColor = (Color)Application.Current.Resources["Primary"];
        btnFirstHalf.TextColor = Colors.White;

        btnSecondHalf.BackgroundColor = Colors.Transparent;
        btnSecondHalf.TextColor = (Color)Application.Current.Resources["PrimaryBlack"];

        // If your BorderlessDatePicker supports clearing:
        // dtpckStart.Date = null;
        // dtpckEnd.Date = null;
    }
    private void ImgBack_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async void ImgLeaveTypeDownArrow_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
    }

    private async void ImgCalendarStart_Tapped(object sender, TappedEventArgs e)
    {
        //await Common.ControlBounceEffect(sender);
        //dtpckStart.Focus();
#if ANDROID
        if (dtpckStart.Handler?.PlatformView is Android.Widget.EditText editText)
        {
            editText.PerformClick();
        }
#else
    dtpckStart.Focus();
#endif

    }

    private async void ImgCalendarEnd_Tapped(object sender, TappedEventArgs e)
    {
        //await Common.ControlBounceEffect(sender);
        //dtpckEnd.Focus();
#if ANDROID
        if (dtpckEnd.Handler?.PlatformView is Android.Widget.EditText editText)
        {
            editText.PerformClick();
        }
#else
    dtpckEnd.Focus();
#endif

    }

    private void LeaveValidation()
    {
        try
        {
        }
        catch (Exception ex)
        {
            Common.ErrorMessage("$Leave ViewLeave-> LeaveValidation");
        }
    }

    private void LeaveType_Clicked(object sender, EventArgs e)
    {
        ResetLeaveType();

        if (sender is Button btnLeaveType)
        {
            if (btnLeaveType.ClassId == "FullDay")
            {
                // Reset half-day state
                isHalfDaySelected = false;
                isFirstHalf = false;

                bdrSubLeaveType.IsVisible = false;

                btnFullday.BackgroundColor =
                    (Color)Common.ResourceDictionary["Primary"];
                btnFullday.TextColor =
                    (Color)Common.ResourceDictionary["PrimaryWhite"];
            }
            else if (btnLeaveType.ClassId == "HalfDay")
            {
                isHalfDaySelected = true;

                bdrSubLeaveType.IsVisible = true;

                btnHalfDay.BackgroundColor =
                    (Color)Common.ResourceDictionary["Primary"];
                btnHalfDay.TextColor =
                    (Color)Common.ResourceDictionary["PrimaryWhite"];

                // Default to First Half
                isFirstHalf = true;

                ResetSubLeaveType();

                btnFirstHalf.BackgroundColor =
                    (Color)Common.ResourceDictionary["Primary"];
                btnFirstHalf.TextColor =
                    (Color)Common.ResourceDictionary["PrimaryWhite"];
            }
        }
    }


    private void ResetLeaveType()
    {
        btnFullday.BackgroundColor = Colors.Transparent;
        btnFullday.TextColor = (Color)Common.ResourceDictionary["PrimaryBlack"];
        btnHalfDay.BackgroundColor = Colors.Transparent; ;
        btnHalfDay.TextColor = (Color)Common.ResourceDictionary["PrimaryBlack"];
    }

    private void ResetSubLeaveType()
    {
        btnFirstHalf.BackgroundColor = Colors.Transparent;
        btnFirstHalf.TextColor = (Color)Common.ResourceDictionary["PrimaryBlack"];
        btnSecondHalf.BackgroundColor = Colors.Transparent; ;
        btnSecondHalf.TextColor = (Color)Common.ResourceDictionary["PrimaryBlack"];
    }

    private void BtnSubLeaveType_Cliked(object sender, EventArgs e)
    {
        ResetSubLeaveType();

        if (sender is Button btnLeaveType)
        {
            if (btnLeaveType.ClassId == "First")
            {
                isFirstHalf = true;

                btnFirstHalf.BackgroundColor =
                    (Color)Common.ResourceDictionary["Primary"];
                btnFirstHalf.TextColor =
                    (Color)Common.ResourceDictionary["PrimaryWhite"];
            }
            else if (btnLeaveType.ClassId == "Second")
            {
                isFirstHalf = false; // IMPORTANT

                btnSecondHalf.BackgroundColor =
                    (Color)Common.ResourceDictionary["Primary"];
                btnSecondHalf.TextColor =
                    (Color)Common.ResourceDictionary["PrimaryWhite"];
            }
        }
    }
    public async void RefreshData()
    {
        await SaveLeaveRequest();
    }
}


public class LeaveTypeModel
{
    public int Id { get; set; }
    public string Name { get; set; }
}