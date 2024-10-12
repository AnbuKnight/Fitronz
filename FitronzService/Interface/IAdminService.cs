using FitronzService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Interface
{
    public interface IAdminService
    {
        Task<int> ApproveOrRejectPartner(Partner partnerDetails);
    }
}
