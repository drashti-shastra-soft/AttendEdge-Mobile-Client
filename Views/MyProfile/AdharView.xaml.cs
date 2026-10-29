using System.Net.Http.Headers;
using AttendEdgeMobileClient.APIService;
using AttendEdgeMobileClient.Model;
using AttendEdgeMobileClient.Utility;

namespace AttendEdgeMobileClient.Views.MyProfile;

public partial class AdharView : ContentView
{
    private byte[]? _selectedPhotoBytes;
    public event EventHandler<bool> SelectedPhotoChanged;
    private bool isOpenselect = false;
    public AdharView()
    {
        InitializeComponent();
        LoadMethods();
    }

    async void LoadMethods()
    {
        await GetUserDocumentDetails();
    }
    public async Task PickAndUploadPhotoAsync()
    {
        try
        {
            var result1 = await MediaPicker.Default.PickPhotoAsync();

            if (result1 == null)
                return;

            using var stream1 = await result1.OpenReadAsync();
            using var ms1 = new MemoryStream();
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

    private async Task UploadPhotoAsync(byte[] imageBytes, string fileName)
    {
        try
        {
            using var client = new HttpClient();
            using var content = new MultipartFormDataContent();

            var imageContent = new ByteArrayContent(imageBytes);
            imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

            content.Add(imageContent, "file", fileName);

            // Replace with your actual API URL
            var url = "https://your-api-endpoint.com/upload";

            var response = await client.PostAsync(url, content);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Photo uploaded successfully!");
            }
            else
            {
                Console.WriteLine($"Upload failed: {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error uploading photo: {ex.Message}");
        }
    }

    private async void BtnUploadFile_Clicked(object sender, EventArgs e)
    {
        await PickAndUploadPhotoAsync();
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

            var mResponse = await new ApiServiceHandler()
                .PostAsync<UserDocument>(Utility.EndPointsUrl.SaveAdharDetail, mUserDocument, Common.AuthToken);

            if (mResponse != null && mResponse.IsSuccess)
            {
                Common.SuccessMessage("Uploaded successfully");
                await GetUserDocumentDetails();
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

    public async Task GetUserDocumentDetails()
    {
        try
        {
            string url = string.Format(Utility.EndPointsUrl.GetAdharDetail, Common.mUser.Id);

            SelectedPhotoChanged?.Invoke(this, true);

            var mResponse = await new ApiServiceHandler().GetAsync<List<UserDocument>>(url, Common.AuthToken);
            if (mResponse != null && mResponse.IsSuccess)
            {
                if (mResponse.Value is List<UserDocument> userDocuments && userDocuments != null)
                {
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
            }
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            SelectedPhotoChanged?.Invoke(this, false);
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
        try
        {
            var result = await FilePicker.PickAsync(new PickOptions
            {
                PickerTitle = "Select a document",
                FileTypes = FilePickerFileType.Pdf, // or custom
            });

            if (result != null)
            {
            }
        }
        catch (Exception ex)
        {
            Common.ErrorMessage(ex.Message);
        }
        //await PickAndUploadPhotoAsync();
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
}