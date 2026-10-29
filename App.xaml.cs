using System.Buffers.Text;
using AttendEdgeMobileClient.Utility;
using AttendEdgeMobileClient.Views.Account;

namespace AttendEdgeMobileClient
{
    public partial class App : Application
    {
        public static string baseUrl;
        public static string FilePathbaseUrl;
        public App()
        {
            InitializeComponent();            
            baseUrl = "https://dev-attend-edge-web-service.shastrasoft.net/";
            Current.UserAppTheme = AppTheme.Light;
            Common.ResourceDictionary = App.Current.Resources.MergedDictionaries.First();
            Common.StyleResource = App.Current.Resources.MergedDictionaries.Last();
            MainPage = new NavigationPage(new LoginPage());
        }

        //protected override Window CreateWindow(IActivationState? activationState)
        //{
        //    //return new Window(new LoginPage());
        //}
    }
}