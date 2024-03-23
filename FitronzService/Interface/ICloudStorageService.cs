using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Interface
{
    public interface ICloudStorageService
    {
        Task<Dictionary<string, string>> UploadFilesToCloudStorage(IFormFileCollection files, string emailAddress, int partnerid, string comments);
        Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage);
        Task DeleteFileAsync(string fileNameForStorage);
        Task<string> GetFileAsync(string fileNameForStorage);
    }
}
