using AttendEdgeMobileClient.APIService;
using AttendEdgeMobileClient.Model;
using AttendEdgeMobileClient.Models;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using AttendEdgeMobileClient.Views.Account;
using Plugin.Fingerprint;
using Plugin.Fingerprint.Abstractions;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AttendEdgeMobileClient.Views.Tapped;

public partial class DashboardView : ContentView, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    private DateTime? _checkInTime;
    public DateTime? CheckInTime
    {
        get { return _checkInTime; }
        set
        {
            if (_checkInTime != value)
            {
                _checkInTime = value;
                OnPropertyChanged();
            }
        }
    }
    private DateTime? _checkOutTime;
    public DateTime? CheckOutTime
    {
        get { return _checkOutTime; }
        set
        {
            if (_checkOutTime != value)
            {
                _checkOutTime = value;
                OnPropertyChanged();
            }
        }
    }
    private DateTime? _breakInTime;
    public DateTime? BreakInTime
    {
        get { return _breakInTime; }
        set
        {
            if (_breakInTime != value)
            {
                _breakInTime = value;
                OnPropertyChanged();
            }
        }
    }
    private DateTime? _breakOutTime;
    public DateTime? BreakOutTime
    {
        get { return _breakOutTime; }
        set
        {
            if (_breakOutTime != value)
            {
                _breakOutTime = value;
                OnPropertyChanged();
            }
        }
    }

    public event EventHandler SwipeCompleted;
    Attendance mAttendance;
    AttendanceLister mAttendanceLister;
    private readonly Action _onApiCompleted;

    // new: the latest record returned from API (could be yesterday)
    //private Attendance mLastKnownRecordForDisplay;

    public DashboardView(Action onApiCompleted)
    {
        InitializeComponent();
        BindingContext = this;
        _onApiCompleted = onApiCompleted;
        mAttendance = new Attendance();
        mAttendanceLister = new AttendanceLister();
        //lstAttendence.ItemsSource = new List<string> { "", "", "", "", "", "", };
        //GetAtendance();
        Loaded += DashboardView_Loaded;
    }

    private async void DashboardView_Loaded(object? sender, EventArgs e)
    {
        await GetAtendance();
    }

    private async Task GetAtendance()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            var mResponse = await new ApiServiceHandler()
                .PostAsync<AttendanceLister>(
                    uri: EndPointsUrl.GetAttendence,
                    mAttendanceLister,
                    Common.AuthToken);

            if (mResponse == null || !mResponse.IsSuccess || mResponse.Value == null)
            {
                mAttendance = null;
                ResetTodayUI();
                return;
            }

            if (mResponse.Value is AttendanceLister attendanceLister)
            {
                if (attendanceLister.List == null ||
                    attendanceLister.List.Count == 0)
                {
                    mAttendance = null;
                    ResetTodayUI();
                    return;
                }

                var today = DateTime.Today;

                // Get ONLY today's record
                var todayRecord = attendanceLister.List
                    .Where(x => x.CheckInTime.Date == today)
                    .OrderByDescending(x => x.Id)
                    .FirstOrDefault();

                // No record for today
                if (todayRecord == null)
                {
                    mAttendance = null;
                    ResetTodayUI();
                    return;
                }

                // Today's record found
                mAttendance = todayRecord;

                CheckInTime = mAttendance.CheckInTime;
                BreakInTime = mAttendance.BreakInTime;
                BreakOutTime = mAttendance.BreakOutTime;
                CheckOutTime = mAttendance.CheckOutTime;


                lblRecordNotFound.IsVisible = false;

                UpdateSwipeText();

                // Load ONLY today's record
                LoadAttendanceHistory(mAttendance, false);
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"DashboardView->GetAtendance: {ex.Message}");
        }
        finally
        {
            _onApiCompleted?.Invoke();
        }
    }

    private void ResetTodayUI()
    {
        lblSwipeButton.Text = "Swipe to Check In";

        CheckInTime = null;
        BreakInTime = null;
        BreakOutTime = null;
        CheckOutTime = null;

        // Clear the attendance list
        lstAttendence.ItemsSource = null;

        lblRecordNotFound.Text = "No Record Found";
        lblRecordNotFound.IsVisible = true;
    }

    private void UpdateSwipeText()
    {
        if (mAttendance == null)
        {
            lblSwipeButton.Text = "Swipe to Check In";
            SwipeThumb.IsEnabled = true;
            return;
        }

        if (mAttendance.CheckInTime == null)
        {
            lblSwipeButton.Text = "Swipe to Check In";
            SwipeThumb.IsEnabled = true;
        }
        else if (mAttendance.BreakInTime == null)
        {
            lblSwipeButton.Text = "Swipe to Break In";
            SwipeThumb.IsEnabled = true;
        }
        else if (mAttendance.BreakOutTime == null)
        {
            lblSwipeButton.Text = "Swipe to Break Out";
            SwipeThumb.IsEnabled = true;
        }
        else if (mAttendance.CheckOutTime == null)
        {
            lblSwipeButton.Text = "Swipe to Check Out";
            SwipeThumb.IsEnabled = true;
        }
        else
        {
            lblSwipeButton.Text = "All Done";
            SwipeThumb.IsEnabled = false;
        }
    }

    // signature changed to accept a record and a boolean
    private void LoadAttendanceHistory(Attendance recordToDisplay, bool isLastRecordOld)
    {
        var displayList = new List<AttendanceDisplayItem>();

        if (recordToDisplay == null)
        {
            // no record to display
            lblRecordNotFound.IsVisible = true;
            lstAttendence.ItemsSource = null;
            return;
        }

        // Build list from the provided record
        if (recordToDisplay.CheckInTime != null)
            displayList.Add(new AttendanceDisplayItem { Title = "Check In", Time = recordToDisplay.CheckInTime, Icon = "icon_checkin.png" });

        if (recordToDisplay.BreakInTime != null)
            displayList.Add(new AttendanceDisplayItem { Title = "Break In", Time = recordToDisplay.BreakInTime, Icon = "icon_cup.png" });

        if (recordToDisplay.BreakOutTime != null)
            displayList.Add(new AttendanceDisplayItem { Title = "Break Out", Time = recordToDisplay.BreakOutTime, Icon = "icon_break_out.png" });

        if (recordToDisplay.CheckOutTime != null)
            displayList.Add(new AttendanceDisplayItem { Title = "Check Out", Time = recordToDisplay.CheckOutTime, Icon = "icon_check_out.png" });

        // If there are no events in the record — show label
        lblRecordNotFound.IsVisible = displayList.Count == 0;

        // Set ItemsSource to the displayList so CollectionView will show it
        lstAttendence.ItemsSource = displayList;
        //clsTodayAttendence.ItemsSource = displayList;

        // If the record is not for today, optionally show a small indicator label so user knows this is from previous date
        if (isLastRecordOld)
        {
            // Example: show last record date somewhere — you can create a label in XAML bound to this.
            //lblRecordNotFound.Text = $"Last record: {recordToDisplay.CheckInTime:MMM dd, yyyy}";
            //lblRecordNotFound.IsVisible = true;
        }
    }

    private async void ImgAdd_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
    }

    private async void IconFilter_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
    }

    private async void brdSwipeToCheckIn_Tapped(object sender, TappedEventArgs e)
    {
        try
        {
            var request = new AuthenticationRequestConfiguration("Authenticate", "Scan your fingerprint");
            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            if (result.Authenticated)
            {

            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Access Denied", "OK");
            }
        }
        catch (Exception ex)
        {
            _ = ex.Message;
        }
    }

    private async void ViewAll_Tapped(object sender, TappedEventArgs e)
    {
        await Common.ControlBounceEffect(sender);
        await Navigation.PushAsync(new AttendenceListPage());
    }

    private async void OnSwiped1(object sender, SwipedEventArgs e)
    {
        if (e.Direction == SwipeDirection.Right)
        {
            // Optional: Small animation to show movement
            await SwipeThumb.TranslateTo(100, 0, 300, Easing.CubicOut);
            await SwipeThumb.TranslateTo(0, 0, 300, Easing.CubicIn);
            var request = new AuthenticationRequestConfiguration("Authenticate", "Scan your fingerprint");
            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            if (result.Authenticated)
            {
                var takeSelfiepage = new TakeSelfiePage(mAttendance);
                takeSelfiepage.TakeSelfieCompleted += async (s1, e1) =>
                {
                    await Task.Delay(300); // wait till page fully visible again
                    await MainThread.InvokeOnMainThreadAsync(async () => await GetAtendance());
                };
                await Navigation.PushAsync(takeSelfiepage);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Access Denied", "OK");
            }

            // await Shell.Current.GoToAsync(nameof(NextPage));
        }
    }

    private async void OnSwiped(object sender, SwipedEventArgs e)
    {
        if (e.Direction != SwipeDirection.Right)
            return;

        // Don't allow swipe when attendance is completed
        if (mAttendance != null &&
            mAttendance.CheckInTime != null &&
            mAttendance.BreakInTime != null &&
            mAttendance.BreakOutTime != null &&
            mAttendance.CheckOutTime != null)
        {
            return;
        }

        try
        {
            // Small animation before authentication
            await SwipeThumb.TranslateTo(100, 0, 300, Easing.CubicOut);
            await SwipeThumb.TranslateTo(0, 0, 300, Easing.CubicIn);

            // Platform-specific biometric handling
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Android can handle directly
                await HandleBiometricAuthenticationAsync();
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                //  iOS must run on the main thread, after animations complete
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await HandleBiometricAuthenticationAsync();
                });
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Error",
                ex.Message,
                "OK");
        }
    }
    private async Task HandleBiometricAuthenticationAsync()
    {
        try
        {
            var request = new AuthenticationRequestConfiguration("Authenticate", "Scan your fingerprint");

            var result = await CrossFingerprint.Current.AuthenticateAsync(request);

            if (result.Authenticated)
            {
                var takeSelfiePage = new TakeSelfiePage(mAttendance);

                takeSelfiePage.TakeSelfieCompleted += async (s1, e1) =>
                {
                    await Task.Delay(300);
                    await MainThread.InvokeOnMainThreadAsync(async () => await GetAtendance());
                };

                await Navigation.PushAsync(takeSelfiePage);
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "Access Denied", "OK");
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage($"DashbordView->HandleBiometricAuthenticationAsync: {ex.Message}");
        }
    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        try
        {
            // Small animation before authentication
            await SwipeThumb.TranslateTo(100, 0, 300, Easing.CubicOut);
            await SwipeThumb.TranslateTo(0, 0, 300, Easing.CubicIn);

            // Platform-specific biometric handling
            if (DeviceInfo.Platform == DevicePlatform.Android)
            {
                // Android can handle directly
                await HandleBiometricAuthenticationAsync();
            }
            else if (DeviceInfo.Platform == DevicePlatform.iOS)
            {
                //  iOS must run on the main thread, after animations complete
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await HandleBiometricAuthenticationAsync();
                });
            }
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
        }
    }
}