using MinimalAPIMoviez.Services;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


public class AzureStorage : IFileStorage
{
    private string connectionString;
    public AzureStorage(IConfiguration config)
    {
        connectionString = config.GetConnectionString("AzureConnection")!;
    }
    public async Task Delete(string? route, string container)
    {
        if (string.IsNullOrEmpty(route))
        {
            return;
        }
        var client = new BlobContainerClient(connectionString, container);
        await client.CreateIfNotExistsAsync();
        var fileName = Path.GetFileName(route);
        var blob = client.GetBlobClient(fileName);
        await blob.DeleteIfExistsAsync();
    }

    //Storing file in Azure 
    public async Task<string> Store(string container, IFormFile file)
    {
        var client = new BlobContainerClient(connectionString, container);
        await client.CreateIfNotExistsAsync();
        //We are giving access to our blob to be readable
        client.SetAccessPolicy(PublicAccessType.Blob);
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        var blob = client.GetBlobClient(fileName);
        BlobHttpHeaders blobHttp = new();
        blobHttp.ContentType = file.ContentType;
        await blob.UploadAsync(file.OpenReadStream(), blobHttp);
        return blob.Uri.ToString();
    }
}