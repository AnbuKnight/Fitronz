using FitronzService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Interface
{
    public interface IPaymentService
    {
        Task<int> AddPaymentInfo(Payment paymentDetails);
    }
}
