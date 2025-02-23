using Xunit;
using Moq;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using FluentAssertions;
using StorageMicroservice.Application.Commands;
using StorageMicroservice.Domain.Entities;
using StorageMicroservice.Infrastructure.Repository;
using StorageMicroservice.Infrastructure.StorageProviders;

public class UploadFileHandlerTests
{
    private readonly Mock<IStorageProvider> _storageProviderMock;
    private readonly Mock<IRepository<FileMetadata>> _fileRepositoryMock;
    private readonly UploadFileHandler _handler;

    public UploadFileHandlerTests()
    {
        _storageProviderMock = new Mock<IStorageProvider>();
        _fileRepositoryMock = new Mock<IRepository<FileMetadata>>();
        _handler = new UploadFileHandler(_storageProviderMock.Object, _fileRepositoryMock.Object);
    }

    [Fact]
    public async Task Handle_ShouldUploadFile_AndSaveMetadata()
    {
        // Arrange
        var fileName = "test.txt";
        var fileMock = new Mock<IFormFile>();
        var memoryStream = new MemoryStream();
        fileMock.Setup(x => x.FileName).Returns(fileName);
        fileMock.Setup(x => x.OpenReadStream()).Returns(memoryStream);
        var fileGuid = Guid.NewGuid();
        var file = new FileMetadata();
        file.Id = fileGuid;
        file.FileName = fileName;
        
        var storageUrl = "https://storage.com/test.txt";
        file.Url= storageUrl;
        _storageProviderMock.Setup(x => x.UploadFileAsync(fileName, It.IsAny<Stream>()))
                            .ReturnsAsync(storageUrl);

        _fileRepositoryMock.Setup(x => x.GetByIdAsync(fileGuid)).ReturnsAsync(file);
        var command = new UploadFileCommand { File = fileMock.Object };
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.FileName.Should().Contain(fileName);
      
    }
}
