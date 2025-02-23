using MediatR;
using StorageMicroservice.Shared.DTOs;
using StorageMicroservice.Shared.Responses;
using System;

public class GetFileByIdQuery : IRequest<Result<FileMetadataDto>>
{
    public Guid Id { get; set; }
}
