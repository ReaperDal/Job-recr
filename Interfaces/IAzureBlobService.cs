namespace Job_Recruitment.Interfaces;

public interface IAzureBlobService
{
    Task<string> InsertImage(Stream stream, string containerName, string blobName);
}