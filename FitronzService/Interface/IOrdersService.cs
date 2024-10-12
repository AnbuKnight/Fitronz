using FitronzService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Interface
{
    public interface IOrdersService
    {
        Task<int> CreateOrder(Orders orderDetails);
        Task<GymDetails> GetGymDetailsForUsers(int partnerId);
        Task<UserOrders> GetUserOrderDetails(int userid);
        Task<List<UserOrders>> GetOrderDetails();
        Task<int> UpdateOrderStatus(OrderStatus orderStatus);
        Task<int> UpdateDaysandStatusForUserCheckin(OrderStatus orderStatus);
    }
}
