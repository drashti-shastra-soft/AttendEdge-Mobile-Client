using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeMobileClient.Utility
{
    public static class AppEnum
    {
        public enum TappedOption
        {
            [Display(Name = "Home")]
            Home = 1,
            [Display(Name = "Report")]
            Report = 2,
            [Display(Name = "Profile")]
            Profile = 3,
            [Display(Name = "Leave")]
            Leave = 4,
            [Display(Name = "Account")]
            Account = 5
        }

        public enum LeaveStatus
        {
            [Display(Name = "Approved")]
            Approved = 1,
            [Display(Name = "Pending")]
            Pending = 2,
            [Display(Name = "Reject")]
            Reject = 3
        }
    }
}
