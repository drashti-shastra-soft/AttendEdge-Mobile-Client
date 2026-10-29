using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static Java.Util.Jar.Attributes;

namespace AttendEdgeMobileClient.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string EmailAddress { get; set; }
    public string MobileNumber { get; set; }
    public bool IsActive { get; set; }
    public string Token { get; set; }
    public List<object> Images { get; set; }
    public int CompanyId { get; set; }
    public string Password { get; set; }
    public object PlainTextPassword { get; set; }
    public int RoleId { get; set; }
    public int CreatedBy { get; set; }
    public object LastModifiedBy { get; set; }
    public object Status { get; set; }
    public object Role { get; set; }
    public object Company { get; set; }

    public string FullName => $"{FirstName} {LastName}";

    [JsonIgnore]
    public string Initials
    {
        get
        {
            if (string.IsNullOrWhiteSpace(FullName)) return "?";
            return string.Join("",
                FullName.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                    .Take(2)
                    .Where(w => !string.IsNullOrEmpty(w))
                    .Select(w => w[0].ToString().ToUpper()));
        }
    }
}
