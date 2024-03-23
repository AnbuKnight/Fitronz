using FitronzService.Interface;
using FitronzService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static Dapper.SqlMapper;

namespace FitronzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IRegisterService _registerPartnerService;
        public RegisterController(IRegisterService registerPartnerService)
        {
            _registerPartnerService=registerPartnerService;
        }

        //[HttpPost(Name = "AddPartner")]
        [HttpPost]
        [Route("AddPartner")]
        public async Task<IActionResult> AddPartner([FromBody] Partner partnerDetails)
        {
            var resultText=string.Empty;
            var result = await _registerPartnerService.AddPartner(partnerDetails);
            if(result==1)
            {
                resultText = "Account created successfully";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while adding the partner";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }
                       
        }

        [HttpGet]
        [Route("GetPartnerDetails")]
        public async Task<IActionResult> GetPartnerDetails(string mobileNumber, string password)
        {
            var resultText = string.Empty;
            var result = await _registerPartnerService.GetPartnerDetails(mobileNumber, password);
            if(result.partner_id!=0)
            {
                resultText = "Partner authorized successfully";
                return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Invalid credentials / Partner not available";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status401Unauthorized };
            }            
        }

        [HttpPost]
        [Route("UpsertGymDetails")]
        public async Task<IActionResult> UpsertGymDetails([FromBody] GymDetails gymDetails)
        {
            var resultText = string.Empty;
            var result = await _registerPartnerService.UpsertGymDetails(gymDetails);
            if (result == -1)
            {
                resultText = "Gym details added/updated successfully";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while adding/updating the partner";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet]
        [Route("GetGymDetails")]
        public async Task<IActionResult> GetGymDetails(int partnerId)
        {
            var resultText = string.Empty;
            var result = await _registerPartnerService.GetGymDetails(partnerId);
            return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
        }

        [HttpPost]
        [Route("DeleteSlot")]
        public async Task<IActionResult> DeleteSlot(DeleteSlotArguments deleteSlotArguments)
        {
            var resultText = string.Empty;
            var result = await _registerPartnerService.DeleteSlot(deleteSlotArguments.partnerId, deleteSlotArguments.slotId);
            if (result > 0)
            {
                resultText = "Slot deleted successfully";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while deleting the slot";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }
        [HttpPost]
        [Route("AddUser")]
        public async Task<IActionResult> AddUser([FromBody] Users userDetails)
        {
            var resultText = string.Empty;
            var result = await _registerPartnerService.AddUser(userDetails);
            if (result == 1)
            {
                resultText = "User created successfully";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while adding the user";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        [HttpGet]
        [Route("GetUserDetails")]
        public async Task<IActionResult> GetUserDetails(string? mobileNumber, string? emailAddress, string password)
        {
            var resultText = string.Empty;
            var result = await _registerPartnerService.GetUserDetails(mobileNumber, emailAddress, password);
            if (result.user_id != 0)
            {
                resultText = "User authorized successfully";
                return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Invalid credentials / User not available";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
