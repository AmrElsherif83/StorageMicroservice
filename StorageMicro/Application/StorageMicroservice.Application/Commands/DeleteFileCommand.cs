using MediatR;
using StorageMicroservice.Shared.Responses;
using System;

public class DeleteFileCommand : IRequest<Result<bool>>
{
    public Guid Id { get; set; }
}
