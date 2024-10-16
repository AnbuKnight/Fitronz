﻿using FitronzService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FitronzService.Interface
{
    public interface IFileService
    {
        Task<int> AddFileDetailsToDB(Files fileDetails);
        Task<List<Files>> GetFileDetails(int partnerId);
        Task<int> DeleteFileDetailsFromDB(string fileName, int partnerId, string action);
        Task DeleteFileCreatedForZipDownload(string storagePath, string fileName);
    }
}
