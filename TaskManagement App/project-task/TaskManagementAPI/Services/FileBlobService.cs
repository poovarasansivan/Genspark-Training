using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public interface IFileBlobService
{
    Task<string> UploadAsync(IFormFile file);
    Task<Stream> DownloadAsync(string blobName);
}


public class FileBlobService : IFileBlobService
{
    private readonly BlobContainerClient _container;

    public FileBlobService(IConfiguration config)
    {
        var connectionString = config["AzureBlob:ConnectionString"];
        var containerName = config["AzureBlob:ContainerName"];

        var blobServiceClient = new BlobServiceClient(connectionString);
        _container = blobServiceClient.GetBlobContainerClient(containerName);

        _container.CreateIfNotExists(PublicAccessType.Blob);
    }

    public async Task<string> UploadAsync(IFormFile file)
    {
        var blobName = $"{Guid.NewGuid()}_{file.FileName}";
        var blobClient = _container.GetBlobClient(blobName);

        await using var stream = file.OpenReadStream();
        await blobClient.UploadAsync(stream, overwrite: true);

        return blobName;
    }

    public async Task<Stream> DownloadAsync(string blobName)
    {
        var blobClient = _container.GetBlobClient(blobName);

        if (!await blobClient.ExistsAsync())
            throw new FileNotFoundException($"Blob '{blobName}' not found");

        var response = await blobClient.DownloadAsync();
        return response.Value.Content;
    }
}
