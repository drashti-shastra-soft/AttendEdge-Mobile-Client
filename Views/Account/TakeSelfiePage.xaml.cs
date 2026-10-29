using AttendEdgeMobileClient.Models;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using CommunityToolkit.Maui.Core;
using SkiaSharp;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text.Json;
#if ANDROID
using Android.Graphics;
#endif


namespace AttendEdgeMobileClient.Views.Account;


public partial class TakeSelfiePage : ContentPage
{
    public byte[] SelfyResult;
    public decimal CheckInLatitude;
    public decimal CheckInLongitude;
    public event EventHandler TakeSelfieCompleted;
    private string locationName = string.Empty;
    public Attendance mAttendance = new Attendance();
    public TakeSelfiePage(Attendance attendance)
    {
        InitializeComponent();

        mAttendance = attendance;

        CameraContainer.IsVisible = false;

        BtnTakeSelfie.IsVisible = true;
        BtnTakeSelfie.IsEnabled = true;

        BtnContinue.IsVisible = true;
    }

    private async void BtnTakeSelfie_Clicked(
        object sender,
        EventArgs e)
    {
        try
        {
            BtnTakeSelfie.IsEnabled = false;

            await Common.ControlBounceEffect(sender);

            // Get available cameras
            var cameras =
                await cameraView.GetAvailableCameras(
                    CancellationToken.None);

            if (cameras == null || cameras.Count == 0)
            {
                await DisplayAlert(
                    "Camera",
                    "No camera available on this device.",
                    "OK");

                return;
            }

            // Find FRONT camera
            var frontCamera =cameras.FirstOrDefault(
                    camera =>camera.Position == CameraPosition.Front);

            if (frontCamera == null)
            {
                await DisplayAlert(
                    "Camera",
                    "Front camera is not available.",
                    "OK");

                return;
            }

            Debug.WriteLine($"Front Camera: {frontCamera}");
            cameraView.SelectedCamera = frontCamera;
            myImage.Source = "img_selfie.png";
            SelfieContainer.IsVisible = false;
            CameraContainer.IsVisible = true;
            cameraView.IsVisible = true;
            await Task.Delay(100);
            await cameraView.StartCameraPreview(CancellationToken.None);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Open Camera Error: {ex}");

            CameraContainer.IsVisible = false;
            cameraView.IsVisible = false;
            SelfieContainer.IsVisible = true;

            await DisplayAlert(
                "Camera Error",
                ex.Message,
                "OK");
        }
        finally
        {
            BtnTakeSelfie.IsEnabled = true;
        }
    }

    private async void CaptureCamera_Tapped(object sender,TappedEventArgs e)
    {
        try
        {
            if (cameraView.IsCameraBusy)
                return;

            await PopLoaderService.ShowAsync();

            Debug.WriteLine(
                "Starting camera capture...");

            using Stream capturedStream = await cameraView.CaptureImage(CancellationToken.None);

            if (capturedStream == null)
            {
                await DisplayAlert(
                    "Error",
                    "Unable to capture image.",
                    "OK");

                return;
            }

            using var memoryStream = new MemoryStream();

            await capturedStream.CopyToAsync(memoryStream);

            byte[] capturedBytes = memoryStream.ToArray();

            if (capturedBytes.Length == 0)
            {
                await DisplayAlert(
                    "Error",
                    "Captured image is empty.",
                    "OK");

                return;
            }

            Debug.WriteLine($"Captured bytes: {capturedBytes.Length}");

            byte[] rotatedBytes = RotateAndUnMirrorImage(capturedBytes);

            if (rotatedBytes != null &&
                rotatedBytes.Length > 0)
            {
                capturedBytes = rotatedBytes;
            }


            Debug.WriteLine(
                $"Rotated bytes: {capturedBytes.Length}");

            await GetCurrentLocation();

            byte[] finalImageBytes = AddLocationToImage(capturedBytes,locationName);

            if (finalImageBytes == null ||
                finalImageBytes.Length == 0)
            {
                finalImageBytes =
                    capturedBytes;
            }

            SelfyResult =
                finalImageBytes;

            Debug.WriteLine(
                $"SelfyResult bytes: {SelfyResult.Length}");

            cameraView.StopCameraPreview();

            CameraContainer.IsVisible = false;

            var imageBytes =
                SelfyResult.ToArray();

            myImage.Source =
                ImageSource.FromStream(
                    () => new MemoryStream(
                        imageBytes));

            myImage.IsVisible = true;
            SelfieContainer.IsVisible = true;
            BtnTakeSelfie.IsVisible = true;
            BtnContinue.IsVisible = true;

            string base64Photo =
                Convert.ToBase64String(
                    SelfyResult);

            Debug.WriteLine(
                $"Base64 Length: {base64Photo.Length}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Capture Error: {ex}");

            await DisplayAlert(
                "Capture Error",
                ex.Message,
                "OK");
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }
    private byte[] RotateAndUnMirrorImage(byte[] imageBytes)
    {
        try
        {
            using var inputStream = new MemoryStream(imageBytes);
            using var bitmap = Android.Graphics.BitmapFactory.DecodeStream(inputStream);

            if (bitmap == null)
                return imageBytes;

            using var matrix = new Android.Graphics.Matrix();

            matrix.PostRotate(-90);
            matrix.PostScale(-1, 1);

            using var rotatedBitmap =
                Android.Graphics.Bitmap.CreateBitmap(
                    bitmap,
                    0,
                    0,
                    bitmap.Width,
                    bitmap.Height,
                    matrix,
                    true);

            using var outputStream =
                new MemoryStream();

            rotatedBitmap.Compress(
                Android.Graphics.Bitmap.CompressFormat.Jpeg,
                95,
                outputStream);

            return outputStream.ToArray();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Rotate/UnMirror Error: {ex}");

            return imageBytes;
        }
    }

    private async Task GetCurrentLocation()
    {
        try
        {
            var location =
                await Geolocation.Default.GetLocationAsync(
                    new GeolocationRequest(
                        GeolocationAccuracy.Medium,
                        TimeSpan.FromSeconds(10)));

            if (location == null)
            {
                Debug.WriteLine(
                    "Location not available.");

                return;
            }

            CheckInLatitude = (decimal)location.Latitude;
            CheckInLongitude = (decimal)location.Longitude;
            Debug.WriteLine( $"Latitude: {CheckInLatitude}");
            Debug.WriteLine( $"Longitude: {CheckInLongitude}");

            try
            {
                var placemarks =
                    await Geocoding.Default
                        .GetPlacemarksAsync(
                            location.Latitude,
                            location.Longitude);

                var placemark =
                    placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    locationName = $"Locality: {placemark.Locality}\n" + $"SubLocality: {placemark.SubLocality}";
                    lblLocationInfo.Text = locationName;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(
                    $"Geocoding Error: {ex}");

                locationName = string.Empty;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Location Error: {ex}");

            locationName = string.Empty;
        }
    }

    private byte[] AddLocationToImage( byte[] imageBytes, string locationText)
    {
        try
        {
            if (imageBytes == null ||
                imageBytes.Length == 0)
            {
                return imageBytes;
            }

            using var originalBitmap =
                SKBitmap.Decode(imageBytes);

            if (originalBitmap == null)
            {
                return imageBytes;
            }


            var imageInfo =
                new SKImageInfo(
                    originalBitmap.Width,
                    originalBitmap.Height);


            using var surface =
                SKSurface.Create(imageInfo);

            if (surface == null)
            {
                return imageBytes;
            }


            var canvas =
                surface.Canvas;

            canvas.Clear( SKColors.Transparent);

            // Draw original image
            canvas.DrawBitmap(
                originalBitmap,
                0,
                0);

            // Add location
            if (!string.IsNullOrWhiteSpace(
                locationText))
            {
                float textSize = Math.Max( 30, originalBitmap.Width / 70f);

                using var paint =
                    new SKPaint
                    {
                        Color = SKColors.Red,
                        TextSize = textSize,
                        IsAntialias = true,
                        Typeface = SKTypeface.FromFamilyName("Arial", SKFontStyle.Bold)
                    };


                float x = 50;

                float y = originalBitmap.Height - 100;


                var lines = locationText.Split( '\n', StringSplitOptions.RemoveEmptyEntries);


                foreach (var line in lines)
                {
                    canvas.DrawText( line, x, y, paint);
                    y += textSize + 15;
                }
            }

            canvas.Flush();
            using var finalImage =
                surface.Snapshot();
            using var data =
                finalImage.Encode(
                    SKEncodedImageFormat.Jpeg,
                    90);

            using var outputStream =
                new MemoryStream();

            data.SaveTo(
                outputStream);

            return outputStream.ToArray();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"AddLocationToImage Error: {ex}");

            return imageBytes;
        }
    }

    private async void CloseCamera_Clicked(
    object sender,
    EventArgs e)
    {
        try
        {
            await Common.ControlBounceEffect(sender);
            cameraView.StopCameraPreview();
            CameraContainer.IsVisible = false;
            SelfieContainer.IsVisible = true;
            BtnTakeSelfie.IsVisible = true;
            BtnTakeSelfie.IsEnabled = true;
            BtnContinue.IsVisible = true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Close Camera Error: {ex}");

            CameraContainer.IsVisible = false;
            SelfieContainer.IsVisible = true;

            BtnTakeSelfie.IsVisible = true;
            BtnTakeSelfie.IsEnabled = true;
            BtnContinue.IsVisible = true;
        }
    }


    private async void BtnContinue_Clicked(object sender, EventArgs e)
    {
        try
        {
            await Common.ControlBounceEffect(sender);
            if (SelfyResult != null)
            {
                var mAttendance = PrepareObject();
                if (mAttendance != null)
                {
                    Debug.WriteLine(JsonSerializer.Serialize(mAttendance, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));

                    await PopLoaderService.ShowAsync();
                    var mResponce = await new APIService.ApiServiceHandler().PostAsync<Attendance, Attendance>(EndPointsUrl.SaveAttendence, mAttendance, Common.AuthToken);
                    if (mResponce != null && mResponce.IsSuccess)
                    {
                        await Navigation.PopAsync();
                        if (mResponce.Value is Attendance attendance && attendance != null)
                        {
                            mAttendance = attendance;
                            TakeSelfieCompleted?.Invoke(attendance, new EventArgs());
                        }
                    }
                    else
                    {
                        await App.Current.MainPage.DisplayAlert( "Error","Unable to save attendance. Please try again.","OK");
                    }
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please take selfie first", "Ok");
            }
        }
        catch (Exception ex)
        {
            //await App.Current.MainPage.DisplayAlert("Error", "Please tack selfie first", "Ok");
            await App.Current.MainPage.DisplayAlert( "Error",ex.Message,"OK");
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }


    private Attendance PrepareObject()
    {
        if (mAttendance == null)
            mAttendance = new Attendance();

        // Common info
        mAttendance.CompanyId = Common.mUser.CompanyId;
        mAttendance.UserId = Common.mUser.Id;
        mAttendance.LastModifiedBy = Common.mUser.Id;
        mAttendance.DeviceInfo = DeviceInfo.Name;
        mAttendance.IPAddress = GetLocalIpAddress();
        mAttendance.LocationDescription = locationName;

        if (SelfyResult == null || SelfyResult.Length == 0)
        {
            return null;
        }

        string selfieBase64 = Convert.ToBase64String(SelfyResult);

        Debug.WriteLine(
            $"Selfie Base64 Length: {selfieBase64.Length}");

        // FIRST ACTION - CHECK IN
        if (mAttendance.CheckInTime == null ||
            mAttendance.CheckInTime == DateTime.MinValue)
        {
            mAttendance.CheckInTime = DateTime.Now;
            mAttendance.LatitudeCheckIn = CheckInLatitude;
            mAttendance.LongitudeCheckIn =  CheckInLongitude;
            mAttendance.CheckInImageBase64 = selfieBase64;
            return mAttendance;
        }

        // SECOND ACTION - BREAK IN
        if (mAttendance.BreakInTime == null)
        {
            mAttendance.BreakInTime = DateTime.Now;
            mAttendance.LatitudeBreakIn = CheckInLatitude;
            mAttendance.LongitudeBreakIn = CheckInLongitude;
            mAttendance.BreakInImageBase64 = selfieBase64;
            return mAttendance;
        }

        // THIRD ACTION - BREAK OUT
        if (mAttendance.BreakOutTime == null)
        {
            mAttendance.BreakOutTime = DateTime.Now;
            mAttendance.LatitudeBreakOut = CheckInLatitude;
            mAttendance.LongitudeBreakOut =  CheckInLongitude;
            mAttendance.BreakOutImageBase64 = selfieBase64;
            return mAttendance;
        }

        // FOURTH ACTION - CHECK OUT
        if (mAttendance.CheckOutTime == null)
        {
            mAttendance.CheckOutTime = DateTime.Now;
            mAttendance.LatitudeCheckOut = CheckInLatitude;
            mAttendance.LongitudeCheckOut = CheckInLongitude;
            mAttendance.CheckOutImageBase64 = selfieBase64;
            return mAttendance;
        }
        return mAttendance;
    }

    public static string GetLocalIpAddress()
    {
        string localIP =
            "Not found";

        try
        {
            foreach (
                var networkInterface
                in NetworkInterface
                    .GetAllNetworkInterfaces())
            {
                if (networkInterface.OperationalStatus != OperationalStatus.Up)
                {
                    continue;
                }


                foreach (var addressInfo in networkInterface .GetIPProperties() .UnicastAddresses)
                {
                    if (addressInfo.Address.AddressFamily == AddressFamily.InterNetwork &&  !IPAddress.IsLoopback(  addressInfo.Address))
                    {
                        return addressInfo.Address
                            .ToString();
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"GetLocalIpAddress Error: {ex}");

            return string.Empty;
        }


        return localIP;
    }

    private async void ImgBack_Tapped(
        object sender,
        TappedEventArgs e)
    {
        try
        {
            await Common.ControlBounceEffect(sender);
            if (CameraContainer.IsVisible)
            {
                cameraView.StopCameraPreview(); 
                CameraContainer.IsVisible = false;
                cameraView.IsVisible = false;
            }
            await Navigation.PopAsync();
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Back Error: {ex}");
        }
    }
    protected override void OnDisappearing()
    {
        try
        {
            if (CameraContainer.IsVisible)
            {
                cameraView.StopCameraPreview();
                cameraView.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"OnDisappearing Camera Error: {ex}");
        }

        base.OnDisappearing();
    }


    private async void BtnContinue_ClickedV1(object sender,EventArgs e)
    {
        try
        {
            await Common.ControlBounceEffect(sender);

            // Check selfie
            if (SelfyResult == null ||
                SelfyResult.Length == 0)
            {
                await App.Current.MainPage.DisplayAlert(
                    "Error",
                    "Please take selfie first",
                    "Ok");

                return;
            }

            if (!await NetworkService.CheckInternetAsync())
                return;

            var mAttendance = PrepareObject();
            if (mAttendance != null)
            {
                //var jsonRequest = JsonSerializer.Serialize( mAttendance, new JsonSerializerOptions 
                //{ 
                //    WriteIndented = true 
                //});

                Debug.WriteLine(JsonSerializer.Serialize(mAttendance, new JsonSerializerOptions
                {
                    WriteIndented = true
                }));
                await PopLoaderService.ShowAsync();
                var mResponce = await new APIService.ApiServiceHandler().PostAsync<Attendance, Attendance>(EndPointsUrl.SaveAttendence, mAttendance, Common.AuthToken);
                if (mResponce != null && mResponce.IsSuccess)
                {
                    await Navigation.PopAsync();
                    if (mResponce.Value is Attendance attendance && attendance != null)
                    {
                        mAttendance = attendance;
                        TakeSelfieCompleted?.Invoke(attendance, new EventArgs());
                    }
                }
                else
                {
                    //error message  
                }
            }

            // Prepare Attendance object
            //var attendance = PrepareObject();

            //if (attendance == null)
            //{
            //    await App.Current.MainPage.DisplayAlert(
            //        "Error",
            //        "Unable to prepare attendance.",
            //        "Ok");

            //    return;
            //}

            // Debug JSON
            //Debug.WriteLine(
            //    JsonSerializer.Serialize(
            //        attendance,
            //        new JsonSerializerOptions
            //        {
            //            WriteIndented = true
            //        }));

            //await PopLoaderService.ShowAsync();

            //// API CALL
            //var mResponce =
            //    await new APIService.ApiServiceHandler()
            //        .PostAsync<Attendance, Attendance>(
            //            EndPointsUrl.SaveAttendence,
            //            attendance,
            //            Common.AuthToken);

            //// SUCCESS
            //if (mResponce != null &&
            //    mResponce.IsSuccess)
            //{
            //    if (mResponce.Value is Attendance savedAttendance &&
            //        savedAttendance != null)
            //    {
            //        mAttendance = savedAttendance;

            //        TakeSelfieCompleted?.Invoke(
            //            savedAttendance,
            //            EventArgs.Empty);
            //    }

            //    await Navigation.PopAsync();
            //}
            //else
            //{
            //    await App.Current.MainPage.DisplayAlert(
            //        "Error",
            //        "Unable to save attendance. Please try again.",
            //        "OK");
            //}
        }
        catch (Exception ex)
        {
            Debug.WriteLine(
                $"Continue Error: {ex}");

            await App.Current.MainPage.DisplayAlert(
                "Error",
                ex.Message,
                "OK");
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }
    private async void BtnContinue_Clicked1(object sender, EventArgs e)
    {
        try
        {
            await Common.ControlBounceEffect(sender);
            if (SelfyResult != null)
            {
                var mAttendance = PrepareObject();
                if (mAttendance != null)
                {
                    //var jsonRequest = JsonSerializer.Serialize(mAttendance, new JsonSerializerOptions
                    //{
                    //    WriteIndented = true
                    //});

                    Debug.WriteLine(JsonSerializer.Serialize(mAttendance, new JsonSerializerOptions
                    {
                        WriteIndented = true
                    }));
                    await PopLoaderService.ShowAsync();
                    var mResponce = await new APIService.ApiServiceHandler().PostAsync<Attendance, Attendance>(EndPointsUrl.SaveAttendence, mAttendance, Common.AuthToken);
                    if (mResponce != null && mResponce.IsSuccess)
                    {
                        await Navigation.PopAsync();
                        if (mResponce.Value is Attendance attendance && attendance != null)
                        {
                            mAttendance = attendance;
                            TakeSelfieCompleted?.Invoke(attendance, new EventArgs());
                        }
                    }
                    else
                    {
                        //error message  
                    }
                }
            }
            else
            {
                await App.Current.MainPage.DisplayAlert("Error", "Please tack selfie first", "Ok");
            }
        }
        catch (Exception)
        {
            await App.Current.MainPage.DisplayAlert("Error", "Please tack selfie first", "Ok");
        }
        finally
        {
            await PopLoaderService.HideAsync();
        }
    }

}
