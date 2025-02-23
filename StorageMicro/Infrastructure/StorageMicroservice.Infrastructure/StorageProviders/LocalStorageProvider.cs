using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StorageMicroservice.Infrastructure.StorageProviders
{
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly string _storagePath;

        public LocalStorageProvider(IOptions<StorageSettings> options)
        {
            _storagePath = options.Value.Local.StoragePath;
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
            }
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            using (var file = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                await fileStream.CopyToAsync(file);
            }
            return filePath;
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            if (!File.Exists(filePath)) throw new FileNotFoundException("File not found", fileName);
            return new FileStream(filePath, FileMode.Open, FileAccess.Read);
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var filePath = Path.Combine(_storagePath, fileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                return true;
            }
            return false;
        }
    }

    public class LocalStorageSettings
    {
        public string StoragePath { get; set; }
    }
}
