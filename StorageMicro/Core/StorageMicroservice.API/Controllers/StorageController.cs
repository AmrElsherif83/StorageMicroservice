using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StorageMicroservice.Application.Commands;
using StorageMicroservice.Application.Queries;

namespace StorageMicroservice.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StorageController : ControllerBase
    {
        private readonly IMediator _mediator;
        public StorageController(IMediator mediator) { _mediator = mediator; }
        [HttpPost("upload")]
        public async Task<IActionResult> Upload([FromForm] UploadFileCommand command)
        { 
            return Ok(await _mediator.Send(command)); 
        }
        
        [HttpGet("list")]
        public async Task<IActionResult> List([FromQuery] string? search, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
         => Ok(await _mediator.Send(new ListFilesQuery(search, page, pageSize)));
        
        [HttpDelete("delete/{id}")] 
        public async Task<IActionResult> Delete(Guid id) => Ok(await _mediator.Send(new DeleteFileCommand { Id = id }));
        
        [HttpGet("download/{id}")] 
        public async Task<IActionResult> Download(Guid id) => File(await _mediator.Send(new DownloadFileQuery { Id = id }), "application/octet-stream", "downloaded_file");

    }
}
