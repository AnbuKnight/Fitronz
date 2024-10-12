using FitronzService.Interface;
using FitronzService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitronzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpPost]
        [Route("ApproveOrRejectPartner")]
        public async Task<IActionResult> ApproveOrRejectPartner([FromBody] Partner partnerDetails)
        {
            Response response = new Response();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // This will return detailed information about the validation errors
            }
            var resultText = string.Empty;
            var result = await _adminService.ApproveOrRejectPartner(partnerDetails);
            if (result != 99)
            {                
                response.ResponseMessage = "Partner details approved/rejected successfully";
                response.StatusCode = (System.Net.HttpStatusCode)StatusCodes.Status200OK;
                return new JsonResult(new { response }) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {                
                response.ResponseMessage = "Error occurred while approving/rejecting the partner";
                response.StatusCode = (System.Net.HttpStatusCode)StatusCodes.Status500InternalServerError;
                return new JsonResult(new { message = resultText }) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }
    }
}
