using AttendEdgeMobileClient.APIService;
using AttendEdgeMobileClient.Model;
using AttendEdgeMobileClient.Services;
using AttendEdgeMobileClient.Utility;
using AttendEdgeMobileClient.Views.MyProfile;
using System.Net.Http.Headers;

namespace AttendEdgeMobileClient.Views.Tapped;

public partial class MyProfileView : ContentView
{
    private byte[]? _selectedPhotoBytes;
    public event EventHandler<bool> SelectedPhotoChanged;
    private bool isOpenselect = false;

    public MyProfileView()
    {
        InitializeComponent();
        SetDefaultView();
    }

    private void SetDefaultView()
    {
        btnPersonal.BackgroundColor = (Color)Common.ResourceDictionary["Primary"];
        btnPersonal.TextColor = (Color)Common.ResourceDictionary["White"];
        //grdViewBind.Add(new PersonalView());
        grdPersonal.IsVisible = true;
        if (Common.mUser != null && Common.mUser.Id > 0)
        {
            //lblName.Text = $"{Common.mUser.FirstName}  {Common.mUser.LastName}";
            lblName.Text = Common.mUser.FullName;
            lblEmail.Text = Common.mUser.EmailAddress;
            lblPhone.Text = Common.mUser.MobileNumber;
        }
    }
    private void ClearSelection()
    {
        btnPersonal.BackgroundColor = Colors.Transparent;
        btnPersonal.TextColor = (Color)Common.ResourceDictionary["PrimaryBlack"];

        btnDocument.BackgroundColor = Colors.Transparent;
        btnDocument.TextColor = (Color)Common.ResourceDictionary["PrimaryBlack"];

        grdPersonal.IsVisible = false;
        grdDocument.IsVisible = false;
    }

    private async void Selection_clicked(object sender, EventArgs e)
    {
        if (sender is Button btnOption)
        {
            ClearSelection();
            if (btnOption.ClassId == "Personal")
            {
                SetDefaultView();
            }
            else if (btnOption.ClassId == "Document")
            {
                btnPlush.IsVisible = true;
                grdDocument.IsVisible = true;
                btnDocument.BackgroundColor = (Color)Common.ResourceDictionary["Primary"];
                btnDocument.TextColor = (Color)Common.ResourceDictionary["White"];
                await GetUserDocumentDetails();
            }
        }
    }

    public async Task GetUserDocumentDetails()
    {
        try
        {
            if (!await NetworkService.CheckInternetAsync())
                return;

            string url = string.Format(Utility.EndPointsUrl.GetAdharDetail, Common.mUser.Id);

            var mResponse = await new ApiServiceHandler().GetAsync<List<UserDocument>>(url, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value is List<UserDocument> userDocuments && userDocuments != null && userDocuments.Count > 0)
                {
                    //lblRecordNotFound.IsVisible = false;
                    foreach (var item in userDocuments)
                    {
                        if (item.MimeType == "image/jpeg" || item.MimeType == "image/png")
                        {
                            //item.Thumbnail = ImageSource.FromUri(new Uri($"{App.FilePathbaseUrl}{item.FilePath}"));
                            item.Thumbnail = ImageSource.FromFile("icon_png.png");
                        }
                        else if (item.MimeType == "application/pdf")
                        {
                            item.Thumbnail = ImageSource.FromFile("icon_pdf.png");
                        }
                        else if (item.MimeType == "application/msword" ||
                                 item.MimeType == "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                        {
                            item.Thumbnail = ImageSource.FromFile("ic_word.png");
                        }
                        else
                        {
                            item.Thumbnail = ImageSource.FromFile("ic_file.png");
                        }
                    }
                    clsUserDocuments.ItemsSource = userDocuments;
                }
                else
                {
                    //lblRecordNotFound.IsVisible = true;
                }
            }
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
        }
    }

    private async void UploadDocument_Tapped(object sender, TappedEventArgs e)
    {
        if (!isOpenselect)
        {
            bdrSelectphoto.IsVisible = true;
            isOpenselect = true;
        }
        else
        {
            bdrSelectphoto.IsVisible = false;
            isOpenselect = false;
        }
    }

    private async void SelectDocument_Tapped(object sender, TappedEventArgs e)
    {
        bdrSelectphoto.IsVisible = false;
        isOpenselect = false;
        FileResult result = null;
        try
        {
            if (sender is Label bdrData && bdrData != null)
            {
                if (bdrData.ClassId == "Photo")
                {
                    result = await MediaPicker.Default.PickPhotoAsync();
                }
                else
                {
                    result = await FilePicker.PickAsync(new PickOptions
                    {
                        PickerTitle = "Select a document",
                        FileTypes = FilePickerFileType.Pdf, // or custom
                    });
                }
            }

            if (result != null)
            {
                using var stream = await result.OpenReadAsync();
                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);

                string base64 = Convert.ToBase64String(ms.ToArray());

                UserDocument mUserDocument = new UserDocument();

                mUserDocument.UserId = Common.mUser.Id;
                mUserDocument.Base64String = base64;
                string fileName = result.FileName;
                string mimeType = GetMimeType(fileName);
                //string mimeType = Path.GetExtension(fileName);
                mUserDocument.MimeType = mimeType;

                string url = string.Format(Utility.EndPointsUrl.SaveUserDocument, Common.mUser.Id);

                var mResponse = await new ApiServiceHandler().PostAsync<UserDocument, UserDocument>(url, mUserDocument, Common.AuthToken);
                if (mResponse != null && mResponse.IsSuccess)
                {
                    Common.SuccessMessage("File has been saved successfully");
                    await GetUserDocumentDetails();
                }
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage(ex.Message);
        }
        //await PickAndUploadPhotoAsync();
    }


    private async void UploadDocument_Tappedv1(object sender, TappedEventArgs e)
    {
        string action = await Application.Current.MainPage.DisplayActionSheet(
            "Select",
            "Cancel",
            null,
            "Select file",
            "Choose photo");

        if (action == "Select file")
        {
            await SelectDocumentAsync(false);
        }
        else if (action == "Choose photo")
        {
            await SelectDocumentAsync(true);
        }
    }

    private async Task SelectDocumentAsync(bool isPhoto)
    {
        FileResult result = null;

        try
        {
            if (isPhoto)
            {
                result = await MediaPicker.Default.PickPhotoAsync();
            }
            else
            {
                result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a document",
                    FileTypes = FilePickerFileType.Pdf
                });
            }

            if (result == null)
                return;

            using var stream = await result.OpenReadAsync();
            using var ms = new MemoryStream();

            await stream.CopyToAsync(ms);

            string base64 = Convert.ToBase64String(ms.ToArray());

            UserDocument mUserDocument = new UserDocument
            {
                UserId = Common.mUser.Id,
                Base64String = base64,
                MimeType = GetMimeType(result.FileName)
            };

            string url = string.Format(
                Utility.EndPointsUrl.SaveUserDocument,
                Common.mUser.Id);

            var mResponse = await new ApiServiceHandler()
                .PostAsync<UserDocument, UserDocument>(
                    url,
                    mUserDocument,
                    Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("File has been saved successfully");
                await GetUserDocumentDetails();
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage(ex.Message);
        }
    }

    private async void SelectPhoto_Tapped(object sender, TappedEventArgs e)
    {
        isOpenselect = false;
        bdrSelectphoto.IsVisible = false;

        try
        {
            await PickAndUploadPhotoAsync();
            //var photo = await MediaPicker.PickPhotoAsync(new MediaPickerOptions());

            //if (photo != null)
            //{
            //}
        }
        catch (Exception ex)
        {

            Common.ErrorMessage(ex.Message);
        }
    }

    public async Task PickAndUploadPhotoAsync()
    {
        try
        {
            var result1 = await MediaPicker.Default.PickPhotoAsync();

            if (result1 == null)
                return;

            var stream1 = await result1.OpenReadAsync();
            var ms1 = new MemoryStream();
            await stream1.CopyToAsync(ms1);
            _selectedPhotoBytes = ms1.ToArray();

            // Show photo in Image control from byte[]
            //imgUplodedPhoto.Source = ImageSource.FromStream(() => new MemoryStream(_selectedPhotoBytes));
            //imgUplodedPhoto.IsVisible = true;

            // Upload the image
            //await UploadPhotoAsync(_selectedPhotoBytes, result1.FileName);
            PostAdharCardDetail(_selectedPhotoBytes, result1.FileName);
        }
        catch (Exception ex)
        {
            // Handle exceptions
            Console.WriteLine($"Error picking or uploading photo: {ex.Message}");
        }
    }

    private async void PostAdharCardDetail(byte[] fileBytes, string fileName)
    {
        try
        {
            // Detect MIME type based on file extension
            string mimeType = GetMimeType(fileName);

            using var content = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(fileBytes);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(mimeType);

            content.Add(fileContent, "file", fileName);

            // Create your model
            UserDocument mUserDocument = new UserDocument();
            //if (mUserDocument != null && mUserDocument.Id == 0)
            //    mUserDocument.CreatedBy = Common.mUser.Id;

            mUserDocument.UserId = Common.mUser.Id;
            mUserDocument.Base64String = Convert.ToBase64String(fileBytes);
            //mUserDocument.FilePath = fileName;
            //mUserDocument.LastModifiedBy = Common.mUser.Id;
            mUserDocument.MimeType = mimeType;

            var mResponse = await new ApiServiceHandler().PostAsync<UserDocument, UserDocument>(Utility.EndPointsUrl.SaveAdharDetail, mUserDocument, Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Uploaded successfully");
                await GetUserDocumentDetails();
            }
            else
            {
                Common.SuccessMessage("Opps somthing wetn wrong..");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    private string GetMimeType(string fileName)
    {
        var extension = Path.GetExtension(fileName)?.ToLower();

        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            ".xls" => "application/vnd.ms-excel",
            ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            ".txt" => "text/plain",
            _ => "application/octet-stream" // default fallback
        };
    }
}