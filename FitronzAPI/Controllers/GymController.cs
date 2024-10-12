using FitronzService.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FitronzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GymController : ControllerBase
    {
        private readonly IGymService _gymService;
        public GymController(IGymService gymService) {
            _gymService = gymService;
        }
        [HttpGet]
        [Route("GetGymList")]
        public async Task<IActionResult> GetGymList()
        {
            var resultText = string.Empty;
            var result = await _gymService.GetGymList();
            if (result != null)
            {
                return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Gym list details not available";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status401Unauthorized };
            }
        }
    }
}
