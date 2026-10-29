using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AttendEdgeMobileClient.Model
{
    public class Holiday 
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public DateTime Date { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsHalfDay { get; set; }
        public bool IsActive { get; set; }
        public int CreatedBy { get; set; }
        public int? LastModifiedBy { get; set; }
        public string Status { get; set; }
    }

    public class HolidayLister
    {
        public List<Holiday> List { get; set; } = new List<Holiday>();

        public Holiday SearchCriteria { get; set; } = new Holiday();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}
