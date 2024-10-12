using FitronzService.Implementation;
using FitronzService.Interface;
using FitronzService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitronzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }
        [HttpPost]
        [Route("CreateOrder")]
        public async Task<IActionResult> CreateOrder([FromBody] Orders orderDetails)
        {
            var resultText = string.Empty;
            var result = await _ordersService.CreateOrder(orderDetails);
            if (result == -1)
            {
                resultText = "Order created successfully";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while creating the order";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet]
        [Route("GetGymDetailsForUsers")]
        public async Task<IActionResult> GetGymDetailsForUsers(int gymId)
        {
            var resultText = string.Empty;
            var result = await _ordersService.GetGymDetailsForUsers(gymId);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Route("GetUserOrderDetails")]
        public async Task<IActionResult> GetUserOrderDetails(int userid)
        {
            var resultText = string.Empty;
            var result = await _ordersService.GetUserOrderDetails(userid);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpGet]
        [Route("GetOrderDetails")]
        public async Task<IActionResult> GetOrderDetails()
        {
            var resultText = string.Empty;
            var result = await _ordersService.GetOrderDetails();
            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [Route("UpdateOrderStatus")]
        public async Task<IActionResult> UpdateOrderStatus([FromBody] OrderStatus orderStatus)
        {
            var resultText = string.Empty;
            var result = await _ordersService.UpdateOrderStatus(orderStatus);
            if (result >0)
            {
                resultText = "Order status updated successfully";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else if (result == 0)
            {
                resultText = "No such order found to update the status";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while updating the order status";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpPost]
        [Route("UpdateDaysandStatusForUserCheckin")]
        public async Task<IActionResult> UpdateDaysandStatusForUserCheckin([FromBody] OrderStatus orderStatus)
        {
            var resultText = string.Empty;
            var result = await _ordersService.UpdateDaysandStatusForUserCheckin(orderStatus);
            if (result > 1)
            {
                resultText = "Days have been decreased by 1 and status updated successfully";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else if (result == 0)
            {
                resultText = "No such order found to update the status and decrement the days";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while decrementing the days";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }
    }
}
