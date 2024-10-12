using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Models
{
    public class GymDetails
    {
        public int partner_id { get; set; }
        public string gym_name { get; set; }
        public string owner_name { get; set; }
        public string mobile_number { get; set; }
        public string email_address { get; set; }
        public float latitude { get; set; }
        public float longitude { get; set; }
        public string? facilities { get; set; }
        public SlotDetails[]? slot_details {  get; set; }
        public DateTime? created_on { get; set; }
        public string? created_by { get; set; }
        public DateTime? updated_on { get; set; }
        public string? updated_by { get; set; }
        public string gym_address { get; set; }
        public string gym_pincode { get; set; }
    }

    public class SlotDetails
    {
        public int slot_id { get; set; }
        public DateTime slot_start_time { get; set; }
        public DateTime slot_end_time { get; set; }
        public string gender { get; set; }
        public GymTypeDetailz[] gym_type_details { get; set; }

    }

    public class GymTypeDetailz
    {
        public int gymtype_id { get; set; }
        public decimal price { get; set; }
        public string gymtype { get; set; }
    }

    public class DeleteSlotArguments
    {
        public int partnerId { get; set; }
        public int slotId { get; set; }
    }
}
