using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace AttendEdgeMobileClient.Model
{
    public class UserDocument
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Base64String { get; set; }
        public string FilePath { get; set; }
        //public int CreatedBy { get; set; }
        //public int? LastModifiedBy { get; set; }
        public string MimeType { get; set; }
        
        [JsonIgnore]  
        public ImageSource FinalImage
        {
            get
            {
                if (string.IsNullOrEmpty(Base64String))
                    return null;

                byte[] imageBytes = Convert.FromBase64String(Base64String);
                return ImageSource.FromStream(() => new MemoryStream(imageBytes));
            }
        }

        [JsonIgnore]
        public ImageSource Thumbnail { get; set; }
    }
}
