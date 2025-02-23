using MediatR;
using StorageMicroservice.Infrastructure.Repository;
using StorageMicroservice.Domain.Entities;
using StorageMicroservice.Shared.DTOs;
using System.Threading;
using System.Threading.Tasks;
using StorageMicroservice.Shared.Responses;

public class GetFileByIdHandler : IRequestHandler<GetFileByIdQuery, Result<FileMetadataDto>>
{
    private readonly IRepository<FileMetadata> _repository;

    public GetFileByIdHandler(IRepository<FileMetadata> repository)
    {
        _repository = repository;
    }

    public async Task<Result<FileMetadataDto>> Handle(GetFileByIdQuery request, CancellationToken cancellationToken)
    {
        var file = await _repository.GetByIdAsync(request.Id);
        if (file == null) return Result<FileMetadataDto>.FailureResult("file not found.");

        return Result<FileMetadataDto>.SuccessResult( new FileMetadataDto
        {
            Id = file.Id,
            FileName = file.FileName,
            FileSize = file.FileSize,
            FileType = file.FileType,
            CreatedAt = file.CreatedAt,
            UpdatedAt = file.UpdatedAt,
            StorageProvider = file.StorageProvider,
            Url = file.Url
        });
    }
}
