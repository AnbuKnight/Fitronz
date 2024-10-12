using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Models
{
    public class Partner
    {
        public int? partner_id { get; set; }
        public string? owner_name { get; set; }
        public string? gym_name { get; set; }
        public string? email_address { get; set; }
        public string? password { get; set; }
        public string? mobile_number { get; set; }
        public string? address { get; set; }
        public string? pincode { get; set; }
        [DefaultValue(false)]
        public bool is_activated { get; set; } = false;
        public float? latitude { get; set; }
        public float? longitude { get; set; }
        public string? facilities { get; set; }
        public int? document { get; set; }
        public DateTime? created_on { get; set; }
        public string? created_by { get; set; }
        public DateTime? updated_on { get; set; }
        public string? updated_by { get; set; }
        public string? admin_action { get; set; }

    }
}
