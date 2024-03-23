using FitronzService.Interface;
using FitronzService.Models;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace FitronzService.Implementation
{
    public class CloudStorageService : ICloudStorageService
    {
        private readonly GoogleCredential googleCredential;
        private readonly StorageClient storageClient;
        private readonly string bucketName;
        private readonly IFileService _fileService;

        public CloudStorageService(IConfiguration configuration, IFileService fileService)
        {
            googleCredential = GoogleCredential.FromFile(configuration.GetValue<string>("GoogleCredentialFile"));
            storageClient = StorageClient.Create(googleCredential);
            bucketName = configuration.GetValue<string>("GoogleCloudStorageBucket");
            _fileService = fileService;
        }

        public async Task<Dictionary<string, string>> UploadFilesToCloudStorage(IFormFileCollection files, string emailAddress, int partnerId, string comments)
        {
            Dictionary<string, string> uploadedFileDetails = new Dictionary<string, string>();
            try
            {
                foreach (var file in files)
                {
                    Console.WriteLine($"uploading file '{file.Name}' to GCP");
                    var uploadResult = UploadFileAsync(file, partnerId+"_"+file.FileName).Result;
                    Console.WriteLine($"uploaded file '{file.Name}' to GCP");
                    uploadedFileDetails.Add(file.FileName, uploadResult);

                    Files fileDetails = new Files();
                    fileDetails.partner_id= partnerId;
                    fileDetails.email_address = emailAddress;
                    fileDetails.file_name = file.FileName;
                    fileDetails.file_link = uploadResult;
                    fileDetails.file_description = comments;
                    fileDetails.created_by = emailAddress;
                    fileDetails.updated_by = emailAddress;
                    await _fileService.AddFileDetailsToDB(fileDetails);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at UploadFilsToCloudStorage()");
                Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                Console.WriteLine("STACK TRACE: " + ex.StackTrace);
            }
           
            return uploadedFileDetails;
        }

        public async Task<string> UploadFileAsync(IFormFile imageFile, string fileNameForStorage)
        {
            using (var memoryStream = new MemoryStream())
            {
                await imageFile.CopyToAsync(memoryStream);
                var dataObject = await storageClient.UploadObjectAsync(bucketName, fileNameForStorage, null, memoryStream);
                return dataObject.MediaLink;
            }
        }

        public async Task DeleteFileAsync(string fileNameForStorage)
        {
            await storageClient.DeleteObjectAsync(bucketName, fileNameForStorage);
        }

        public async Task<string> GetFileAsync(string fileNameForStorage)
        {
            string filePath = string.Empty;
            try
            {
                var tt = await storageClient.GetObjectAsync(bucketName, fileNameForStorage);
                filePath= tt.MediaLink;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception at GetFileAsync()");
                Console.WriteLine("EXCEPTION MESSAGE: " + ex.Message);
                Console.WriteLine("INNER EXCEPTION: " + ex.InnerException);
                Console.WriteLine("STACK TRACE: " + ex.StackTrace);
            }
            return filePath;
        }
    }
}
