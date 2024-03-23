using FitronzService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Interface
{
    public interface IRegisterService
    {
        Task<int> AddPartner(Partner partnerDetails);
        Task<Partner> GetPartnerDetails(string mobileNumber, string password);
        Task<int> UpsertGymDetails(GymDetails gymDetails);
        Task<List<SlotDetails>> GetGymDetails(int partnerId);
        Task<int> DeleteSlot(int partnerId, int slotId);
        Task<int> AddUser(Users userDetails);
        Task<Users> GetUserDetails(string mobileNumber, string emailAddress, string password);
    }
}
