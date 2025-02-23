using Xunit;
using Moq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;
using StorageMicroservice.Application.Queries;
using System.Web.Mvc;
using StorageMicroservice.Infrastructure.StorageProviders;
using StorageMicroservice.Infrastructure.Repository;
using StorageMicroservice.Domain.Entities;
using StorageMicroservice.Shared.Responses;
namespace StorageMicroservice.Tests.Handlers;
public class DownloadFileHandlerTests
{
    private readonly Mock<IStorageProvider> _storageProviderMock;
    private readonly Mock<IRepository<FileMetadata>> _repo;
    private readonly DownloadFileHandler _handler;

    public DownloadFileHandlerTests()
    {
        _storageProviderMock = new Mock<IStorageProvider>();
        _repo = new Mock<IRepository<FileMetadata>>();
        _handler = new DownloadFileHandler(_repo.Object,_storageProviderMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldReturnFileStream_WhenFileExists()
    {
        // Arrange
        var fileName = "test.txt";
       var fileGuid = Guid.NewGuid();   
        var stream = new MemoryStream();
        var file = new FileMetadata();
        file.Id = fileGuid;
        file.FileName= fileName;
        _storageProviderMock.Setup(x => x.DownloadFileAsync(fileName))
                            .ReturnsAsync(stream);
        _repo.Setup(x => x.GetByIdAsync(fileGuid)).ReturnsAsync(file);
        var query = new DownloadFileQuery { Id= fileGuid };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeOfType<byte[]>();
        
        //var fileStreamResult =  new System.Web.Mvc.FileContentResult(result, "application/octet-stream");
        //fileStreamResult.FileDownloadName.Should().Be(fileName);
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenFileDoesNotExist()
    {
        // Arrange
        var fileName = "nonexistent.txt";
        var fileGuid = Guid.NewGuid();
        var file = new FileMetadata();
        file.Id = fileGuid;
        file.FileName = fileName;
        _storageProviderMock.Setup(x => x.DownloadFileAsync(fileName))
                            .ReturnsAsync((Stream?)null);
        _repo.Setup(x => x.GetByIdAsync(fileGuid)).ReturnsAsync((FileMetadata?)null);
        var query = new DownloadFileQuery { Id = fileGuid };


        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.Should().BeNullOrEmpty();
    }
}
