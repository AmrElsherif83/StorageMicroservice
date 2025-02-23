using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using StorageMicroservice.Infrastructure.StorageProviders;

public class StorageProviderFactory : IStorageProvider
{
    private readonly IOptionsMonitor<StorageSettings> _storageSettings;
    private readonly IServiceProvider _serviceProvider;

    public StorageProviderFactory(IOptionsMonitor<StorageSettings> storageSettings, IServiceProvider serviceProvider)
    {
        _storageSettings = storageSettings;
        _serviceProvider = serviceProvider;
    }

    private IStorageProvider GetStorageProvider()
    {
        var settings = _storageSettings.CurrentValue;

        return settings.Provider switch
        {
            "S3" => _serviceProvider.GetRequiredService<S3StorageProvider>(),
            "AzureBlob" => _serviceProvider.GetRequiredService<AzureBlobStorageProvider>(),
            _ => _serviceProvider.GetRequiredService<LocalStorageProvider>(),
        };
    }

    public Task<string> UploadFileAsync(string fileName, Stream fileStream)
        => GetStorageProvider().UploadFileAsync(fileName, fileStream);

    public Task<Stream> DownloadFileAsync(string fileName)
        => GetStorageProvider().DownloadFileAsync(fileName);

    public Task<bool> DeleteFileAsync(string fileName)
        => GetStorageProvider().DeleteFileAsync(fileName);
}
