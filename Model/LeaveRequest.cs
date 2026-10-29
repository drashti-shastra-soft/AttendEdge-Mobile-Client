using System.Text.Json.Serialization;

namespace AttendEdgeMobileClient.Model
{
    //public class LeaveRequest
    //{
    //    public int Id { get; set; }
    //    public int CompanyId { get; set; }
    //    public int UserId { get; set; }
    //    public string Username { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //    public string LeaveType { get; set; }
    //    public string HalfType { get; set; }
    //    public string Reason { get; set; }
    //    public string Status { get; set; }
    //    public string AdminNote { get; set; }
    //    public int ProcessedByAdminId { get; set; }
    //    public bool IsHalfDay { get; set; }
    //    public int CreatedBy { get; set; }
    //    public DateTime CreatedDate { get; set; }
    //    public int? LastModifiedBy { get; set; }
    //    public DateTime LastModifiedDate { get; set; }
    //    public DateTime? DeletedByDate { get; set; }
    //    public int? DeletedBy { get; set; }

    //    //Extra
    //    [JsonIgnore]
    //    public int LeaveStatus { get; set; }
    //    [JsonIgnore]
    //    public string LeaveRequestType { get; set; }
    //    [JsonIgnore]
    //    public double TotalDays
    //    {
    //        get
    //        {
    //            // Base full days (inclusive)
    //            double total = (EndDate - StartDate).Days + 1;

    //            // Subtract 0.5 for each half-day
    //            if (IsHalfDay) total -= 0.5;

    //            return total;
    //        }
    //    }
    //    [JsonIgnore]
    //    public bool IsHalfTypeVisibility
    //    {
    //        get { return IsHalfDay ? true : false; }
    //    }
    //}
    public class LeaveRequest
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public string HalfType { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string AdminNote { get; set; }
        public int? ProcessedByAdminId { get; set; }
        public string LeaveStatus { get; set; }
        public bool IsHalfDay { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }

        [JsonIgnore]
        public string LeaveRequestType { get; set; }

        [JsonIgnore]
        public double TotalDays
        {
            get
            {
                // Base full days (inclusive)
                double total = (EndDate - StartDate).Days + 1;

                // Subtract 0.5 for each half-day
                if (IsHalfDay) total -= 0.5;

                return total;
            }
        }
        [JsonIgnore]
        public bool IsHalfTypeVisibility
        {
            get { return IsHalfDay ? true : false; }
        }


    }

    public class LeaveRequestLister
    {
        public List<LeaveRequest> List { get; set; } = new List<LeaveRequest>();

        public LeaveRequest SearchCriteria { get; set; } = new LeaveRequest();

        public Pagination Pagination { get; set; } = new Pagination();
    }

}
