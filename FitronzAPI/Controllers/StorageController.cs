using FitronzService.Implementation;
using FitronzService.Interface;
using FitronzService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
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
        public async Task<IActionResult> UploadFiles(List<IFormFile> Files, string emailAddress, int partnerId, string comments, string googleReviewLink)
        {
            Console.WriteLine("Inside GetFile");

            Console.WriteLine("File count through HTTP request: " + HttpContext.Request.Form.Files.Count.ToString());
            if (HttpContext.Request.Form.Files.Count > 0)
            {
                IFormFileCollection formFiles = HttpContext.Request.Form.Files;
                var uploadResult = await _cloudStorageService.UploadFilesToCloudStorage(formFiles, emailAddress, partnerId, comments, googleReviewLink);
                return Ok(uploadResult);
            }
            return Ok();
        }


        //[HttpGet]
        //[Route("GetDocuments")]
        //public async Task<IActionResult> GetDocuments(string emailAddress)
        //{
        //    var result = await _cloudStorageService.GetFileAsync("Test1-20230929211144.jpg");

        //    return Ok(result);
        //}

        //[HttpGet]
        //[Route("TestStorageController")]
        //public async Task<IActionResult> TestStorageController()
        //{
        //    var result = "Controller being hit successfully";

        //    return Ok(result);
        //}

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

        //[HttpPost("upload")]
        //public async Task<IActionResult> Upload(IFormFile file)
        //{
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    var path = Path.Combine("/var/www/fitronzapi/storage", file.FileName);

        //    using (var stream = new FileStream(path, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }
        //    Console.WriteLine("Uploade file path: " + path);
        //    return Ok(new { filePath = path });
        //}

        private readonly string _storagePath = "/var/www/fitronzapi/storage";

        [HttpGet("DownloadSingleFile")]
        public IActionResult DownloadSingleFile(string fileName, string emailAddress)
        {
            var filePath = Path.Combine(_storagePath,  fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileBytes = System.IO.File.ReadAllBytes(filePath);
            var mimeType = "application/octet-stream"; // Set appropriate MIME type
            return File(fileBytes, mimeType, fileName);
        }

        //[HttpGet("DownloadSingleFile")]
        //public IActionResult DownloadSingleFile(string fileName, int partnerId)
        //{
        //    var files = Directory.GetFiles(_storagePath, $"{"Partner_ID_" + partnerId + "_" + fileName}*")
        //                        .Select(Path.GetFileName)
        //                        .ToList();

        //    if (files.Count == 0)

        //    {
        //        return NotFound();
        //    }

        //    return Ok(files);
        //}

        [HttpGet("DownloadAllFiles")]
        public IActionResult DownloadAllFiles(string emailAddress)
        {
            var files = Directory.GetFiles(_storagePath, $"{emailAddress.Split('@')[0]}*")
                                .Select(Path.GetFileName)
                                .ToList();

            if (files.Count == 0)
            {
                return NotFound();
            }

            var zipPath = Path.Combine(_storagePath, emailAddress.Split('@')[0] + "_" +"fitronz_files.zip");

            using (var zipStream = new FileStream(zipPath, FileMode.Create))
            using (var archive = new ZipArchive(zipStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var filePath = Path.Combine(_storagePath, file);
                    archive.CreateEntryFromFile(filePath, file);
                }
            }

            var zipBytes = System.IO.File.ReadAllBytes(zipPath);
            _fileService.DeleteFileCreatedForZipDownload(_storagePath, emailAddress.Split('@')[0] + "_" + "fitronz_files.zip");
            return File(zipBytes, "application/zip", "fitronz_files.zip");
        }

        //public void DeleteFileCreatedForZipDownload(string fileName)
        //{
        //    var filePath = Path.Combine(_storagePath, fileName);

        //    if (System.IO.File.Exists(filePath))
        //    {
        //        System.IO.File.Delete(filePath);
        //    }
        //}

        [HttpDelete("DeleteSingleFile")]
        public async Task<IActionResult> DeleteSingleFileAsync(string fileName, int partnerId, string emailAddress)
        {
            var filePath = Path.Combine(_storagePath,  fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            System.IO.File.Delete(filePath);
            await _fileService.DeleteFileDetailsFromDB(fileName, partnerId, "single_delete");
            return Ok(new { message = $"File {fileName} deleted successfully." });
        }

        [HttpDelete("DeleteAllFiles")]
        public async Task<IActionResult> DeleteAllFilesAsync(int partnerId, string emailAddress)
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
            await _fileService.DeleteFileDetailsFromDB("", partnerId, "delete_all");
            return Ok(new { message = "Files deleted successfully.", deletedFiles = files });
        }

        [HttpGet("get-files/{searchString}")]
        public async Task<IActionResult> GetFiles(string searchString)
        {
            try
            {
                // Define the storage directory path
                var directoryPath = "/var/www/fitronzapi/storage";

                // Get all files in the directory
                var files = Directory.GetFiles(directoryPath)
                                      .Where(f => Path.GetFileName(f).Contains(searchString))
                                      .Select(f => Path.GetFileName(f))
                                      .ToList();

                // If no files match, return not found
                if (files.Count == 0)
                {
                    return NotFound("No files found matching the search string.");
                }

                // Return the list of matching file names
                return Ok(files);
            }
            catch (System.Exception ex)
            {                
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("get-file/{fileName}")]
        public async Task<IActionResult> GetFile(string fileName)
        {
            try
            {
                // Define the file path on the server
                var filePath = Path.Combine("/var/www/fitronzapi/storage", fileName);

                // Check if the file exists
                if (!System.IO.File.Exists(filePath))
                {
                    return NotFound("File not found");
                }

                // Read the file content as a byte array
                var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);

                // Return the file as a response (with appropriate content type, assuming JPG images)
                return File(fileBytes, "image/jpeg"); // You can modify the MIME type based on your file type
            }
            catch (Exception ex)
            {                
                return StatusCode(500, "Internal server error");
            }
        }
    }
}

