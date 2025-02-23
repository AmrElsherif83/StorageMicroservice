using MediatR;
using StorageMicroservice.Shared.DTOs;
using StorageMicroservice.Shared.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Application.Queries
{
    public class ListFilesQuery(string? search, int page, int pageSize) : IRequest<PagingResult<FileMetadataDto>>
    {
        public string Search { get; set; } = search;

        public int Page { get; set; } = page;

        public int PageSize { get; set; } = pageSize;
    }
}
