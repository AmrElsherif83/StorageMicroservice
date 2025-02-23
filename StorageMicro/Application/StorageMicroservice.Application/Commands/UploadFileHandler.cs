using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using StorageMicroservice.Domain.Entities;
using StorageMicroservice.Infrastructure.Repository;
using StorageMicroservice.Infrastructure.StorageProviders;
using StorageMicroservice.Shared.DTOs;
using StorageMicroservice.Shared.Responses;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace StorageMicroservice.Application.Commands
{
    public class UploadFileHandler : IRequestHandler<UploadFileCommand, Result<FileMetadataDto>>
    {
        private readonly IStorageProvider _storageProvider;
        private readonly IRepository<FileMetadata> _repository;

        public UploadFileHandler(IStorageProvider storageProvider, IRepository<FileMetadata> repository)
        {
            _storageProvider = storageProvider;
            _repository = repository;
        }
        public async Task<Result<FileMetadataDto>> Handle(UploadFileCommand request, CancellationToken cancellationToken)
        {            
            var fileName = $"{Guid.NewGuid()}_{request.File.FileName}";
            using var stream = request.File.OpenReadStream();
            var url = await _storageProvider.UploadFileAsync(fileName, stream);

            var fileMetadata = new FileMetadata
            {
                Id = Guid.NewGuid(),
                FileName = fileName,
                CreatedAt = DateTime.UtcNow,
                FileType = System.IO.Path.GetExtension(request.File.FileName),
                FileSize = request.File.Length,
                StorageProvider = _storageProvider.GetType().Name,
                Url = url
            };

            await _repository.AddAsync(fileMetadata);

            return Result<FileMetadataDto>.SuccessResult(fileMetadata.Adapt<FileMetadataDto>(), "File uploaded successfully");
        }
    }
}