using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StorageMicroservice.Application.Queries
{
    public class DownloadFileQuery : IRequest<byte[]> { public Guid Id { get; set; } }
}
