using Android.App;
using Android.Content.PM;
using Android.OS;
using Plugin.Fingerprint;

namespace AttendEdgeMobileClient
{
    [Activity(
                Label = "AttendEdge ",
                Theme = "@style/Maui.SplashTheme",
                MainLauncher = true,
                LaunchMode = LaunchMode.SingleTop,
                ConfigurationChanges = ConfigChanges.ScreenSize |
                ConfigChanges.Orientation |
                ConfigChanges.UiMode |
                ConfigChanges.ScreenLayout |
                ConfigChanges.SmallestScreenSize |
                ConfigChanges.Density, ScreenOrientation = ScreenOrientation.Unspecified)]
 
    public class MainActivity : MauiAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            CrossFingerprint.SetCurrentActivityResolver(() => this);
            if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            {
                // Set background color of the status bar
                Window.SetStatusBarColor(Android.Graphics.Color.ParseColor("#3085FE")); // Purple color

                // Optional: Set icons to dark or light
                // For light icons on dark background (default)
                Window.DecorView.SystemUiVisibility = 0;

                // For dark icons on light background:
                // Window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            }

        }
    }
}
