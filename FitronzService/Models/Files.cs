using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Models
{
    public class Files
    {
        public int partner_id { get; set; }
        public string file_name { get; set; }
        public string file_link { get; set; }
        public string file_description { get; set; }
        public string created_by { get; set; }
        public DateTime created_on { get; set; }
        public string updated_by { get; set; }
        public string email_address { get; set; }
    }
}
