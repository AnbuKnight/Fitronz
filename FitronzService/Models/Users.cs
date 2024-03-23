using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Models
{
    public class Users
    {
        public int user_id { get; set; }
        public string user_name { get; set; }
        public int user_age { get; set; }
        public string user_gender { get; set; }
        public string user_address { get; set; }
        public string user_pincode { get; set; }
        public string user_mobile_number { get; set; }
        public string user_email_address { get; set; }
        public string user_password { get; set; }
        public string user_profile_picture { get; set; }        
        public DateTime? created_on { get; set; }
        public string? created_by { get; set; }
        public DateTime? updated_on { get; set; }
        public string? updated_by { get; set; }
    }
}
