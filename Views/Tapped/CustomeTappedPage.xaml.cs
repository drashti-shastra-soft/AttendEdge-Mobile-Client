using AttendEdgeMobileClient.APIService;
using AttendEdgeMobileClient.CustomeControls;
using AttendEdgeMobileClient.Models;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using AttendEdgeMobileClient.Views.Account;
using Microsoft.Maui.Controls.Shapes;

namespace AttendEdgeMobileClient.Views.Tapped;

public partial class CustomeTappedPage : FlyoutPage
{
    private int selectedIndex = 2;
    private int previousIndex = 2;

    private readonly string[] blueIcons =
    {
        "icon_tab_three_blue.png",
        "icon_tab_two_blue.png",
        "icon_tab_one_blue.png",
        "icon_tab_five_blue.png",
        "icon_tab_four_blue.png",
    };

    private readonly string[] blackIcons =
    {
        "icon_tab_three_white.png",
        "icon_tab_two_white.png",
        "icon_tab_one_white.png",
        "icon_tab_five_white.png",
        "icon_tab_four_white.png",
    };

    public (int, string) data;

    private BottomBarShapeDrawable bottomBarDrawable;

    public CustomeTappedPage()
    {
        InitializeComponent();

        // User information
        lblUserName.Text = Common.mUser.FullName;
        lblInitial.Text = Common.mUser.Initials;

        // Bottom bar
        bottomBarDrawable = new BottomBarShapeDrawable
        {
            SelectedIndex = selectedIndex
        };

        BottomCurve.Drawable = bottomBarDrawable;

        // Generate bottom tabs
        GenerateTabs();

        // Load Dashboard initially
        _ = LoadTabAsync(selectedIndex);
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
    }

    private async Task LoadTabAsync(int index)
    {
        try
        {
            await PopLoaderService.ShowAsync();
            await Task.Delay(100);

            View viewToLoad = index switch
            {
                // Profile
                0 => new MyProfileView(),
                // Leave Balance
                1 => new LeaveBalanceView(),
                // Dashboard
                2 => new DashboardView(OnDashboardApiDone),
                // Apply Leave
                3 => new LeaveView(),
                // Holidays
                4 => new HolidayView(),
                _ => new Label
                {
                    Text = "Unknown Tab"
                }
            };

            MainContent.Content = viewToLoad;
            await Task.Delay(300);
            await PopLoaderService.HideAsync();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine(
                $"LoadTabAsync Error: {ex}");

            await PopLoaderService.HideAsync();
        }
    }

    private void GenerateTabs()
    {
        TabGrid.Children.Clear();

        for (int i = 0; i < blackIcons.Length; i++)
        {
            int index = i;

            View tabView;

            if (i == selectedIndex)
            {
                var imageButton = new ImageButton
                {
                    Source = blackIcons[i],

                    HeightRequest = 30,
                    WidthRequest = 30,

                    BackgroundColor = Colors.Transparent,

                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,

                    Command = new Command(async () =>
                    {
                        await OnTabSelectedAsync(index);
                    })
                };

                var border = new Border
                {
                    Padding = 10,

                    HeightRequest = 60,
                    WidthRequest = 60,

                    StrokeShape = new RoundRectangle
                    {
                        CornerRadius = 30
                    },

                    BackgroundColor = Colors.DodgerBlue,

                    TranslationY = -10,

                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,

                    Content = imageButton
                };

                tabView = border;
            }
            else
            {
                tabView = new ImageButton
                {
                    Source = blueIcons[i],

                    BackgroundColor = Colors.Transparent,

                    HeightRequest = 30,
                    WidthRequest = 30,

                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,

                    Command = new Command(async () =>
                    {
                        await OnTabSelectedAsync(index);
                    })
                };
            }

            Grid.SetColumn(tabView, i);

            TabGrid.Children.Add(tabView);
        }
    }

    private async Task OnTabSelectedAsync(int index)
    {
        if (index == selectedIndex)
            return;

        previousIndex = selectedIndex;
        selectedIndex = index;
        // Update bottom bar
        bottomBarDrawable.SelectedIndex = index;
        BottomCurve.Invalidate();
        // Regenerate tabs
        GenerateTabs();
        // Load selected page
        await LoadTabAsync(index);
    }

    private void MenuButton_Clicked(
        object sender,
        EventArgs e)
    {
        // Open native FlyoutPage
        IsPresented = true;
    }

    private async void ProfileMenu_Tapped(
        object sender,
        TappedEventArgs e)
    {
        // Close flyout
        IsPresented = false;

        if (selectedIndex != 0)
        {
            await OnTabSelectedAsync(0);
        }
    }

    private async void LeaveBalanceMenu_Tapped(
        object sender,
        TappedEventArgs e)
    {
        // Close flyout
        IsPresented = false;

        if (selectedIndex != 1)
        {
            await OnTabSelectedAsync(1);
        }
    }

    private async void DashboardMenu_Tapped(
        object sender,
        TappedEventArgs e)
    {
        // Close flyout
        IsPresented = false;

        if (selectedIndex != 2)
        {
            await OnTabSelectedAsync(2);
        }
    }

    private async void LeaveMenu_Tapped(
        object sender,
        TappedEventArgs e)
    {
        // Close flyout
        IsPresented = false;

        if (selectedIndex != 3)
        {
            await OnTabSelectedAsync(3);
        }
    }

    private async void HolidayMenu_Tapped(
        object sender,
        TappedEventArgs e)
    {
        // Close flyout
        IsPresented = false;

        if (selectedIndex != 4)
        {
            await OnTabSelectedAsync(4);
        }
    }

    private async void ChangePasswordMenu_Tapped(
        object sender,
        TappedEventArgs e)
    {
        // Close flyout
        IsPresented = false;

        await Navigation.PushAsync(
            new ChangePasswordPage());
    }

    private async void LogoutMenu_Tapped(object sender,TappedEventArgs e)
    {
        // Close flyout
        IsPresented = false;
        bool result = await DisplayAlert(
            "Logout",
            "Are you sure you want to logout?",
            "Yes",
            "No");
        if (!result)
            return;

        Common.AuthToken = string.Empty;

        // Clear stored token
        Preferences.Remove("AuthToken");

        await Navigation.PopToRootAsync();
    }

    public async void OnDashboardApiDone()
    {
        await PopLoaderService.HideAsync();
    }

    protected override bool OnBackButtonPressed()
    {
        if (IsPresented)
        {
            IsPresented = false;

            return true;
        }

        if (selectedIndex == 2)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                bool exit = await DisplayAlert(
                    "Exit App",
                    "Are you sure you want to exit?",
                    "Yes",
                    "No");

                if (exit)
                {
#if ANDROID
                    Android.OS.Process.KillProcess(
                        Android.OS.Process.MyPid());
#endif
                }
            });

            return true;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await OnTabSelectedAsync(previousIndex);
        });

        return true;
    }
}