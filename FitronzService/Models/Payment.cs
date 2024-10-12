using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Models
{
    public class Payment
    {
        public int partner_id { get; set; }
        public string account_number { get; set; }
        public string account_holder_name { get; set; }
        public string bank_name { get; set; }
        public string ifsc_code { get; set; }
        public DateTime created_on { get; set; }
        public string created_by { get; set; }
        public DateTime updated_on { get; set; }
        public string updated_by { get; set; }
    }
}
