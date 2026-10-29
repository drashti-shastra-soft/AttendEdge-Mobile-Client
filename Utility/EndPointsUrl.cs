namespace AttendEdgeMobileClient.Utility
{
    public class EndPointsUrl
    {
        public const string Authentication = "api/Authentication";
        public const string ForgotPassword = "api/ForgotPassword";
        public const string ChangePassword = "api/ChangePassword";
        public const string GetAllLeave = "api/LeaveRequest/GetAll";
        public const string AddLeave = "api/LeaveRequest";
        public const string GetLeave = "api/LeaveRequest";
        public const string GetById = "api/LeaveRequest/id/{0}";  //Id


        #region [ Holiday ]
        public const string GetAllHoliday = "api/Holiday/GetAll";
        public const string GetHoliday = "api/Holiday";
        #endregion

        #region [ Attendence ]
        public const string GetAttendence = "api/Attendance/GetAll";
        public const string SaveAttendence = "api/Attendance";
        #endregion

        public const string SaveAdharDetail = "api/UserDocument/Create";
        public const string GetAdharDetail = "api/UserDocument/UserId/{0}"; //UserId
        public const string SaveUserDocument = "api/UserDocument/Create";
    }
}
