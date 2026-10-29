using AttendEdgeMobileClient.Utility;
using Mopups.Animations;
using Mopups.Enums;
using Mopups.Pages;
using Mopups.Services;

namespace AttendEdgeMobileClient.PopupPages;

//public partial class FilterPopup : PopupPage
//{
//	public FilterPopup()
//	{
//		InitializeComponent();
//	}
//}

    //private bool _isApproved;
    //private bool _isOneDay;
    //private bool _isTwoDay;
    //private bool _isThreeDayOrMore;
    //private bool _isCasualLeave;
    //private bool _isSickLeave;
    //private bool _isPaidLeave;
    //private bool _isHalf;
public partial class FilterPopup : PopupPage
{

    private bool _isApproved = false;
    private bool _isRejected = false;
    private bool _isOneDay = false;
    private bool _isTwoDay = false;
    private bool _isThreeDayOrMore = false;
    private bool _isCasualLeave = false;
    private bool _isSickLeave = false;
    private bool _isPaidLeave = false;
    private bool _isHalf = false;

    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    public event EventHandler<FilterResult>? FilterApplied;


        //Animation = new MoveAnimation
        //{
        //    PositionIn = MoveAnimationOptions.Bottom,
        //    PositionOut = MoveAnimationOptions.Bottom,
        //    DurationIn = 300,
        //    DurationOut = 300
        //};
    public FilterPopup(DateTime? startDate = null,DateTime? endDate = null)
    {
        InitializeComponent();

        StartDate = startDate ?? DateTime.Today;
        EndDate = endDate ?? DateTime.Today;

        if (EndDate < StartDate)
        {
            EndDate = StartDate;
        }

        dtpckStart.Date = StartDate;
        dtpckEnd.Date = EndDate;

        // Initial validation
        //dtpckEnd.MinimumDate = dtpckStart.Date;
    }


    private async void BtnClose_Tapped(object sender,TappedEventArgs e)
    {
        await MopupService.Instance.PopAsync();
    }
    private void CheckBox_tapped(object sender, TappedEventArgs e)
    {
        if (sender is HorizontalStackLayout lblCheckBox)
        {
            if (lblCheckBox.Children[1] is Label lbl && lbl.ClassId == "Approve")
            {
                ImgReject.Source = AppConstraints.UnCheck;
                ImgApprove.Source = AppConstraints.Check;
                _isApproved = true;
                _isRejected = false;
            }
            else if (lblCheckBox.Children[1] is Label lbl1 && lbl1.ClassId == "Reject")
            {
                ImgReject.Source = AppConstraints.Check;
                ImgApprove.Source = AppConstraints.UnCheck;
                _isApproved = false;
                _isRejected = true;
            }
            else if (lblCheckBox.Children[1] is Label lblOneDay && lblOneDay.ClassId == "OneDay")
            {
                ImgOneDay.Source = AppConstraints.Check;
                ImgTwoDay.Source = AppConstraints.UnCheck;
                ImgThreeDay.Source = AppConstraints.UnCheck;
                _isOneDay = true;
            }
            else if (lblCheckBox.Children[1] is Label lblTwoDay && lblTwoDay.ClassId == "TwoDay")
            {
                ImgTwoDay.Source = AppConstraints.Check;
                ImgOneDay.Source = AppConstraints.UnCheck;
                ImgThreeDay.Source = AppConstraints.UnCheck;
                _isTwoDay = true;
            }
            else if (lblCheckBox.Children[1] is Label lblThreeDay && lblThreeDay.ClassId == "ThreeDay")
            {
                ImgThreeDay.Source = AppConstraints.Check;
                ImgTwoDay.Source = AppConstraints.UnCheck;
                ImgOneDay.Source = AppConstraints.UnCheck;
                _isThreeDayOrMore = true;
            }
            else if (lblCheckBox.Children[1] is Label lblCasualLeave && lblCasualLeave.ClassId == "CasualLeave")
            {
                ImgCasualLeave.Source = AppConstraints.Check;
                ImgSickLeave.Source = AppConstraints.UnCheck;
                ImgPaidLeave.Source = AppConstraints.UnCheck;
                _isCasualLeave = true;
            }
            else if (lblCheckBox.Children[1] is Label lblSickLeave && lblSickLeave.ClassId == "SickLeave")
            {
                ImgSickLeave.Source = AppConstraints.Check;
                ImgCasualLeave.Source = AppConstraints.UnCheck;
                ImgPaidLeave.Source = AppConstraints.UnCheck;
                _isSickLeave = true;
            }
            else if (lblCheckBox.Children[1] is Label lblPaidLeave && lblPaidLeave.ClassId == "PaidLeave")
            {
                ImgPaidLeave.Source = AppConstraints.Check;
                ImgSickLeave.Source = AppConstraints.UnCheck;
                ImgCasualLeave.Source = AppConstraints.UnCheck;
                _isPaidLeave = true;
            }
            else if (lblCheckBox.Children[1] is Label lblHalfDay && lblHalfDay.ClassId == "HalfDay")
            {
                ImgHalfDay.Source = AppConstraints.Check;
                ImgFullDay.Source = AppConstraints.UnCheck;
                _isHalf = true;
            }
            else if (lblCheckBox.Children[1] is Label lblFullDay && lblFullDay.ClassId == "FullDay")
            {
                ImgFullDay.Source = AppConstraints.Check;
                ImgHalfDay.Source = AppConstraints.UnCheck;
            }
        }
    }

    private async void SetReset_filterClciked(
        object sender,
        EventArgs e)
    {
        if (sender is not Button button)
            return;


        if (button.ClassId == "Reset")
        {
            ResetFilter();
            return;
        }


        if (button.ClassId == "Apply")
        {
            var result = new FilterResult
            {
                StartDate = dtpckStart.Date,
                EndDate = dtpckEnd.Date,

                IsApproved = _isApproved,
                IsRejected = _isRejected,

                IsOneDay = _isOneDay,
                IsTwoDay = _isTwoDay,
                IsThreeDayOrMore = _isThreeDayOrMore,

                IsCasualLeave = _isCasualLeave,
                IsSickLeave = _isSickLeave,
                IsPaidLeave = _isPaidLeave,

                IsHalfDay = _isHalf
            };
            // Close popup first
            if (MopupService.Instance.PopupStack.Contains(this))
            {
                await MopupService.Instance.PopAsync();
            }

            FilterApplied?.Invoke(this, result);

        }
    }


    private void ResetFilter()
    {
        ImgApprove.Source =
        ImgReject.Source =
        ImgOneDay.Source =
        ImgTwoDay.Source =
        ImgThreeDay.Source =
        ImgCasualLeave.Source =
        ImgSickLeave.Source =
        ImgPaidLeave.Source =
        ImgHalfDay.Source =
        ImgFullDay.Source =
            AppConstraints.UnCheck;


        _isApproved = false;
        _isRejected = false;

        _isOneDay = false;
        _isTwoDay = false;
        _isThreeDayOrMore = false;

        _isCasualLeave = false;
        _isSickLeave = false;
        _isPaidLeave = false;

        _isHalf = false;
    }

    private void dtpckStart_DateSelected(object sender, DateChangedEventArgs e)
    {
        StartDate = e.NewDate;

        if (EndDate < StartDate)
        {
            EndDate = StartDate;
            dtpckEnd.Date = EndDate;
        }
    }

    private void dtpckEnd_DateSelected(object sender, DateChangedEventArgs e)
    {
        if (e.NewDate < StartDate)
        {
            //Common.ErrorMessage("End date cannot be before start date.");
            dtpckEnd.Date = StartDate;
            EndDate = StartDate;
            return;
        }

        EndDate = e.NewDate;
    }
}

//public class FilterResult
//{
//    public DateTime StartDate { get; set; }
//    public DateTime EndDate { get; set; }

//    public bool IsApproved { get; set; }

//    public bool IsOneDay { get; set; }
//    public bool IsTwoDay { get; set; }
//    public bool IsThreeDayOrMore { get; set; }

//    public bool IsCasualLeave { get; set; }
//    public bool IsSickLeave { get; set; }
//    public bool IsPaidLeave { get; set; }

//    public bool IsHalfDay { get; set; }
//}


public class FilterResult
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public bool IsApproved { get; set; }
    public bool IsRejected { get; set; }

    public bool IsOneDay { get; set; }
    public bool IsTwoDay { get; set; }
    public bool IsThreeDayOrMore { get; set; }

    public bool IsCasualLeave { get; set; }
    public bool IsSickLeave { get; set; }
    public bool IsPaidLeave { get; set; }

    public bool IsHalfDay { get; set; }
}
