using Acr.UserDialogs;
using AttendEdgeMobileClient.CustomeControls;
using AttendEdgeMobileClient.Handlers;
using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using SimpleToolkit.SimpleShell;
using Syncfusion.Maui.Core.Hosting;
using Syncfusion.Maui.Toolkit.Hosting;


namespace AttendEdgeMobileClient
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCamera()
                .UseUserDialogs()
                .ConfigureSyncfusionToolkit()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("Mulish-Bold.ttf", "Bold");
                    fonts.AddFont("Mulish-ExtraBold.ttf", "ExtraBold");
                    fonts.AddFont("Mulish-Regular.ttf", "Regular");
                    fonts.AddFont("Mulish-SemiBold.ttf", "SemiBold");
                });
            builder.UseSimpleShell();
            builder.Services.AddSingleton<BottomBarShapeDrawable>();
#if Android
            builder.Services.AddSingleton(typeof(IFingerprint), CrossFingerprint.Current);
            Permissions.RequestAsync<Permissions.Camera>();
            Permissions.RequestAsync<Permissions.StorageRead>();
            Permissions.RequestAsync<Permissions.StorageWrite>();  
#endif

#if DEBUG
            builder.Logging.AddDebug();
#endif

            BoderlessEntryHandler.RemoveBorder();
            return builder.Build();
        }
    }
}
