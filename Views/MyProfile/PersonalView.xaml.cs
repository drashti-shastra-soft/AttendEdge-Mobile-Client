using AttendEdgeMobileClient.Utility;

namespace AttendEdgeMobileClient.Views.MyProfile;

public partial class PersonalView : ContentView
{
    public PersonalView()
    {
        InitializeComponent();
        if (Common.mUser != null && Common.mUser.Id > 0)
        {
            lblName.Text = $"{Common.mUser.FirstName}  {Common.mUser.LastName}";
            lblEmail.Text = Common.mUser.EmailAddress;
            lblPhone.Text = Common.mUser.MobileNumber;
        }
    }
}