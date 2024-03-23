using FitronzService.Implementation;
using FitronzService.Interface;
using FitronzService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace FitronzAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly ICloudStorageService _cloudStorageService;
        private readonly IFileService _fileService;
        public StorageController(ICloudStorageService cloudStorageService, IFileService fileService)
        {
            _cloudStorageService = cloudStorageService;
            _fileService = fileService;
        }
        [Consumes("multipart/form-data")]
        [HttpPost]
        [Route("UploadFiles")]
        public async Task<IActionResult> UploadFiles(List<IFormFile> Files, string emailAddress, int partnerId, string comments)
        {
            Console.WriteLine("Inside GetFile");

            Console.WriteLine("File count through HTTP request: " + HttpContext.Request.Form.Files.Count.ToString());
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                IFormFileCollection formFiles = HttpContext.Request.Form.Files;
                var uploadResult = await _cloudStorageService.UploadFilesToCloudStorage(formFiles, emailAddress, partnerId, comments);
                return Ok(uploadResult);
            }
            return Ok();
        }


        [HttpGet]
        [Route("GetDocuments")]
        public async Task<IActionResult> GetDocuments(string emailAddress)
        {
            var result = await _cloudStorageService.GetFileAsync("Test1-20230929211144.jpg");

            return Ok(result);
        }

        [HttpGet]
        [Route("TestStorageController")]
        public async Task<IActionResult> TestStorageController()
        {
            var result = "Controller being hit successfully";

            return Ok(result);
        }

        [HttpGet]
        [Route("GetFileDetails")]
        public async Task<IActionResult> GetFileDetails(int partnerId)
        {
            var resultText = string.Empty;
            var result = await _fileService.GetFileDetails(partnerId);
            if (result.Count != 0)
            {
                resultText = "File details fetched successfully";
                return new ObjectResult(result) { StatusCode = StatusCodes.Status200OK };
            }
            else
            {
                resultText = "Error occurred while fetching the file details";
                return new ObjectResult(resultText) { StatusCode = StatusCodes.Status500InternalServerError };
            }            
        }
    }
}

