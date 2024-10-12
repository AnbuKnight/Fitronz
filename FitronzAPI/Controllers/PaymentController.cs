using FitronzService.Interface;
using FitronzService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitronzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        public PaymentController(IPaymentService paymentService) {
            _paymentService = paymentService;

        }

        [HttpPost]
        [Route("AddPaymentInfo")]
        public async Task<IActionResult> AddPaymentInfo([FromBody] Payment paymentDetails)
        {
            var resultText = string.Empty;
            var result = await _paymentService.AddPaymentInfo(paymentDetails);
            if (result == 1)
            {
                resultText = "Payment info created successfully";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while adding the payment info";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }
    }
}
