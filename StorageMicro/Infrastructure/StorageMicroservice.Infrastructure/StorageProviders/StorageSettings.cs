public class StorageSettings
{
    public string Provider { get; set; } = "Local";
    public S3StorageSettings S3 { get; set; }
    public AzureBlobStorageSettings AzureBlob { get; set; }
    public LocalStorageSettings Local { get; set; }
}

public class S3StorageSettings
{
    public string AccessKey { get; set; }
    public string SecretKey { get; set; }
    public string Region { get; set; }
    public string BucketName { get; set; }
}

public class AzureBlobStorageSettings
{
    public string ConnectionString { get; set; }
    public string ContainerName { get; set; }
}

public class LocalStorageSettings
{
    public string StoragePath { get; set; }
}
