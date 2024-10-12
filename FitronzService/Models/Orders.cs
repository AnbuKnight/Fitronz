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
        //public int slot_id { get; set; }
        //public int gymtype_id { get; set; }
        public int persons { get; set; }
        public int days { get; set; }
        public UserOrderSlotDetails[]? user_slot_details { get; set; }
        public string status { get; set; }
        public DateTime order_start_date { get; set; }
        public DateTime order_end_date { get; set; }
        public DateTime? created_on { get; set; }
        public string? created_by { get; set; }
        public DateTime? updated_on { get; set; }
        public string? updated_by { get; set; }
    }

    public class UserOrderSlotDetails
    {
        public int slot_id { get; set; }
        public DateTime slot_start_time { get; set; }
        public DateTime slot_end_time { get; set; }        
        public UserOrderGymTypeDetailz[]? user_order_gym_type_details { get; set; }

    }

    public class UserOrderGymTypeDetailz
    {
        public int gymtype_id { get; set; }
        public string gender { get; set; }
        public decimal price { get; set; }
        public int quantity { get; set; }
    }

    public class UserOrders
    {
        public int order_id { get; set; }
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
        public string user_name { get; set; }
        public int user_age { get; set; }
        public string user_gender { get; set; }
    }

    public class OrderStatus
    {
        public int order_id { get; set; }
        public int partner_id { get; set; }
        public string status { get; set; }
        public string? updated_by { get; set; }
        public int user_id { get; set; }
    }

}

