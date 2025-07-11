using Azure.Storage.Blobs;

namespace FitnessTracking.Services
{
    public class BlobService
    {
        private readonly string _connectionString;
        private readonly string _containerName;
        public BlobService(string connectionString, string containerName)
        {
            _connectionString = connectionString;
            _containerName = containerName;
        }
        public async Task<string> UploadFileAsync(string fileName, Stream fileStream, string contentType)
        {
            var blobClient = new BlobContainerClient(_connectionString, _containerName);
            await blobClient.CreateIfNotExistsAsync();
            var blob = blobClient.GetBlobClient(fileName);
            await blob.UploadAsync(fileStream, overwrite: true);

            await blob.SetHttpHeadersAsync(new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = contentType
            });

            return blob.Uri.ToString();
        }

        public async Task<Stream> DownloadFileAsync(string fileName)
        {
            var blobClient = new BlobContainerClient(_connectionString, _containerName);
            var blob = blobClient.GetBlobClient(fileName);

            if (await blob.ExistsAsync())
            {
                var response = await blob.DownloadAsync();
                return response.Value.Content;
            }
            throw new FileNotFoundException("File not found in blob storage.");
        }
    }
}