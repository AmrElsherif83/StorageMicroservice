using Mapster;
using MediatR;
using Microsoft.EntityFrameworkCore;
using StorageMicroservice.Domain.Entities;
using StorageMicroservice.Infrastructure.Repository;
using StorageMicroservice.Shared.DTOs;
using StorageMicroservice.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Application.Queries
{
    public class ListFilesHandler : IRequestHandler<ListFilesQuery, PagingResult<FileMetadataDto>>
    {
        private readonly IRepository<FileMetadata> _dbContext;
        public ListFilesHandler(IRepository<FileMetadata> dbContext) => _dbContext = dbContext;

        public async Task<PagingResult<FileMetadataDto>> Handle(ListFilesQuery request, CancellationToken cancellationToken)
        {
            var query = (await _dbContext.GetAllAsync()).AsQueryable();
            if (!string.IsNullOrEmpty(request.Search))
            {
                query = query.Where(f => f.FileName.Contains(request.Search));
            }

            var totalItems =  query.Count();
            var items =  query.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();

            return  PagingResult<FileMetadataDto>.Create(items.Adapt<List<FileMetadataDto>>(),request.Page,request.PageSize,totalItems);

        }
    }
}
