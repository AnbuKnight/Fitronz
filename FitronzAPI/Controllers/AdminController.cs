using FitronzService.Implementation;
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

                if(partnerDetails.admin_action== "REJECT")
                {
                    DeleteAllFilesAsync(partnerDetails.email_address);
                }
                else
                {
                    if (partnerDetails.files_to_be_deleted != string.Empty && partnerDetails.files_to_be_deleted != null)
                    {
                        var fileToBeDeletedArray = partnerDetails.files_to_be_deleted.Split(',');
                        if (fileToBeDeletedArray.Length > 0)
                        {
                            foreach (var rejectedFile in fileToBeDeletedArray)
                            {
                                DeleteSingleFileAsync(rejectedFile, partnerDetails.email_address);
                            }
                        }
                    }
                }

                return new JsonResult(new { response }) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {                
                response.ResponseMessage = "Error occurred while approving/rejecting the partner";
                response.StatusCode = (System.Net.HttpStatusCode)StatusCodes.Status500InternalServerError;
                return new JsonResult(new { message = resultText }) { StatusCode = StatusCodes.Status500InternalServerError };
            }

        }

        private readonly string _storagePath = "/var/www/fitronzapi/storage";
        /// <summary>
        /// This method is used to delete the files from Hostinger storage during the admin approval workflow
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="partnerId"></param>
        /// <returns></returns>
        [HttpDelete("DeleteFile")]
        public async Task<IActionResult> DeleteSingleFileAsync(string fileName, string emailAddress)
        {
            var filePath = Path.Combine(_storagePath,  fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            System.IO.File.Delete(filePath);            
            return Ok(new { message = $"File {fileName} deleted successfully." });
        }

        [HttpDelete("DeleteAllFiles")]
        public async Task<IActionResult> DeleteAllFilesAsync(string emailAddress)
        {
            var files = Directory.GetFiles(_storagePath, $"{emailAddress.Split('@')[0]}*")
                                .Select(Path.GetFileName)
                                .ToList();

            if (files.Count == 0)
            {
                return NotFound(new { message = "No files available in the server for this partner to be deleted." });
            }

            foreach (var file in files)
            {
                var filePath = Path.Combine(_storagePath, file);
                System.IO.File.Delete(filePath);
            }            
            return Ok(new { message = "Files deleted successfully.", deletedFiles = files });
        }
    }
}
