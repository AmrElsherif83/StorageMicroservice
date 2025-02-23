
using MediatR;
using StorageMicroservice.Application.Queries;
using StorageMicroservice.Infrastructure.StorageProviders;
using StorageMicroservice.Infrastructure.Repository;
using StorageMicroservice.Domain.Entities;


public class DownloadFileHandler : IRequestHandler<DownloadFileQuery, byte[]>
{
    private readonly IRepository<FileMetadata> _fileRepository;
    private readonly IStorageProvider _storageProvider;
    private IStorageProvider @object;

    

    public DownloadFileHandler(IRepository<FileMetadata> fileRepository, IStorageProvider storageProvider)
    {
        _fileRepository = fileRepository;
        _storageProvider = storageProvider;
    }

    public async Task<byte[]> Handle(DownloadFileQuery request, CancellationToken cancellationToken)
    {
        var fileMetadata = await _fileRepository.GetByIdAsync(request.Id);
        if (fileMetadata == null)
        {
            return null;
        }

        var stream=  (await _storageProvider.DownloadFileAsync(fileMetadata.FileName));
        byte[] bytes;
        List<byte> totalStream = new();
        byte[] buffer = new byte[32];
        int read;
        while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
        {
            totalStream.AddRange(buffer.Take(read));
        }
        bytes = totalStream.ToArray();
        return bytes;
    }
}
