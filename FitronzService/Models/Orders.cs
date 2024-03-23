using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Models
{
    public class Orders
    {
        //public Users user { get; set; }
        //public GymDetails gymDetails { get; set; }
        //public int persons {  get; set; }
        //public int days { get; set; }
        //public int gymId { get; set; }
        //public string status { get; set; }
        //public DateTime orderStartDate { get; set; }
        //public DateTime orderEndDate { get; set; }

        public int user_id { get; set; }
        public int partner_id { get; set; }
        public int slot_id { get; set; }
        public int gymtype_id { get; set; }
        public int persons { get; set; }
        public int days { get; set; }
        public string status { get; set; }
        public DateTime order_start_date { get; set; }
        public DateTime order_end_date { get; set; }
        public DateTime? created_on { get; set; }
        public string? created_by { get; set; }
        public DateTime? updated_on { get; set; }
        public string? updated_by { get; set; }
    }

    public class UserOrders
    {
        public int user_id { get; set; }
        public int persons { get; set; }
        public int days { get; set; }
        public string status { get; set; }
        public DateTime order_start_date { get; set; }
        public DateTime order_end_date { get; set; }
        public DateTime? created_on { get; set; }
        public string? created_by { get; set; }
        public DateTime? updated_on { get; set; }
        public string? updated_by { get; set; }
        public GymDetails gymDetails { get; set; }
    }

}

