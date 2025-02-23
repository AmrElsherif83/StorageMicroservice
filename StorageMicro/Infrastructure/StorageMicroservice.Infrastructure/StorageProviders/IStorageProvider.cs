using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Infrastructure.StorageProviders
{
    public interface IStorageProvider
    { 
        Task<string> UploadFileAsync(string fileName, Stream fileStream); 
        Task<Stream> DownloadFileAsync(string fileName); 
        Task<bool> DeleteFileAsync(string fileName); 
    }
}
