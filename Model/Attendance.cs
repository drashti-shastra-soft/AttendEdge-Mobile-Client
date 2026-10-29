using AttendEdgeMobileClient.Model;
using System.Text.Json.Serialization;

namespace AttendEdgeMobileClient.Models
{
    public class Attendance
    {
        public int Id { get; set; }
        public int? CompanyId { get; set; }
        public string Company { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public DateTime CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime? BreakInTime { get; set; }
        public DateTime? BreakOutTime { get; set; }
        public decimal LatitudeCheckIn { get; set; }
        public decimal LongitudeCheckIn { get; set; }
        public decimal? LatitudeCheckOut { get; set; }
        public decimal? LongitudeCheckOut { get; set; }
        public decimal? LatitudeBreakIn { get; set; }
        public decimal? LongitudeBreakIn { get; set; }
        public decimal? LatitudeBreakOut { get; set; }
        public decimal? LongitudeBreakOut { get; set; }
        public string LocationDescription { get; set; } = string.Empty;
        public string DeviceInfo { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string CheckInImageBase64 { get; set; } = string.Empty;
        public string CheckOutImageBase64 { get; set; } = string.Empty;
        public string BreakInImageBase64 { get; set; } = string.Empty;
        public string BreakOutImageBase64 { get; set; } = string.Empty;
        public int? LastModifiedBy { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }

        ///// <summary>Total break duration (handles a break that is still running).</summary>
        //[JsonIgnore]
        //public TimeSpan TotalBreakTime
        //{
        //    get
        //    {
        //        if (BreakInTime == null)
        //            return TimeSpan.Zero;

        //        // If break has ended use BreakOutTime, otherwise break is still running
        //        var breakEnd = BreakOutTime ?? CheckOutTime ?? DateTime.Now;
        //        var breakTime = breakEnd - BreakInTime.Value;

        //        return breakTime > TimeSpan.Zero ? breakTime : TimeSpan.Zero;
        //    }
        //}

        ///// <summary>Total working time = (CheckOut or Now) - CheckIn - Break.</summary>
        //[JsonIgnore]
        //public TimeSpan TotalWorkingTime
        //{
        //    get
        //    {
        //        var endTime = CheckOutTime ?? DateTime.Now;
        //        var total = endTime - CheckInTime - TotalBreakTime;

        //        return total > TimeSpan.Zero ? total : TimeSpan.Zero;
        //    }
        //}

        ///// <summary>Display text, e.g. "8h 30m".</summary>
        //[JsonIgnore]
        //public string TotalWorkingHours =>
        //    $"{(int)TotalWorkingTime.TotalHours}h {TotalWorkingTime.Minutes:D2}m";

    }
    public class AttendanceLister
    {
        public List<Attendance> List { get; set; } = new List<Attendance>();

        public Attendance SearchCriteria { get; set; } = new Attendance();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
