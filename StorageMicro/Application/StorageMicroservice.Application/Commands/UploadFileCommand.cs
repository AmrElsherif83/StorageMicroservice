using MediatR;
using Microsoft.AspNetCore.Http;
using StorageMicroservice.Shared.DTOs;
using StorageMicroservice.Shared.Responses;

namespace StorageMicroservice.Application.Commands
{
    public class UploadFileCommand : IRequest<Result<FileMetadataDto>>
    {
        public IFormFile File { get; set; }
    }
}
