using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Shared.Responses
{
    public class PagingResult<T> : Result<IEnumerable<T>>
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public static PagingResult<T> Create(IEnumerable<T> data, int pageNumber, int pageSize, int totalCount, string message = "Success") =>
            new PagingResult<T>
            {
                Success = true,
                Data = data,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                Message = message
            };
    }
}
