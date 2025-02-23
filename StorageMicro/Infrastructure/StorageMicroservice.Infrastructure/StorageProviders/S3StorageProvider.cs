using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Threading.Tasks;

namespace StorageMicroservice.Infrastructure.StorageProviders
{
    public class S3StorageProvider : IStorageProvider
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3StorageProvider(IOptions<S3StorageSettings> options, IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
            _bucketName = options.Value.BucketName;
        }

        public async Task<string> UploadFileAsync(string fileName, Stream fileStream)
        {
            var putRequest = new PutObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName,
                InputStream = fileStream,
                AutoCloseStream = true
            };

            await _s3Client.PutObjectAsync(putRequest);
            return $"https://{_bucketName}.s3.amazonaws.com/{fileName}";
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var getRequest = new GetObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            var response = await _s3Client.GetObjectAsync(getRequest);
            return response.ResponseStream;
        }

        public async Task<bool> DeleteFileAsync(string fileName)
        {
            var deleteRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = fileName
            };

            await _s3Client.DeleteObjectAsync(deleteRequest);
            return true;
        }
    }

    public class S3StorageSettings
    {
        public string BucketName { get; set; }
    }
}
