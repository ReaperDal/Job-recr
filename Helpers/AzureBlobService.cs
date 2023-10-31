using Azure.Storage.Blobs;
using Job_Recruitment.Interfaces;

namespace Job_Recruitment.Helpers;

public class AzureBlobService : IAzureBlobService
{
    private readonly BlobServiceClient _blobServiceClient;

    public AzureBlobService(IConfiguration configuration)
    {
        _blobServiceClient = new BlobServiceClient(configuration["Azure:ConnectionString"]);
    }

    public async Task<string> InsertImage(Stream stream, string containerName, string blobName)
    {
        var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
        await containerClient.CreateIfNotExistsAsync();

        var blobClient = containerClient.GetBlobClient(blobName);
        await blobClient.UploadAsync(stream, true);

        return blobClient.Uri.ToString();
    }
}
