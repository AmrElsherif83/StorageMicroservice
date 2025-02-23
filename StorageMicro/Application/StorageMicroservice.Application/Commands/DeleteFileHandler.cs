using MediatR;
using StorageMicroservice.Infrastructure.Repository;
using StorageMicroservice.Domain.Entities;
using System.Threading;
using System.Threading.Tasks;
using StorageMicroservice.Infrastructure.StorageProviders;
using StorageMicroservice.Shared.Responses;

public class DeleteFileHandler : IRequestHandler<DeleteFileCommand, Result<bool>>
{
    private readonly IRepository<FileMetadata> _repository;
    private readonly IStorageProvider _storageProvider;

    public DeleteFileHandler(IRepository<FileMetadata> repository, IStorageProvider storageProvider)
    {
        _repository = repository;
        _storageProvider = storageProvider;
    }

    public async Task<Result<bool>> Handle(DeleteFileCommand request, CancellationToken cancellationToken)
    {
        var fileMetadata = await _repository.GetByIdAsync(request.Id);
        if (fileMetadata == null) return Result<bool>.FailureResult("file not found.");

        await _storageProvider.DeleteFileAsync(fileMetadata.FileName);
        await _repository.DeleteAsync(fileMetadata.Id);
        return Result<bool>.SuccessResult(true);
    }
}
