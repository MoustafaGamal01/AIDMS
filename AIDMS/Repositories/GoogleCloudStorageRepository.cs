using AIDMS.Repositories;
using Google.Cloud.Storage.V1;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

public class GoogleCloudStorageRepository : IGoogleCloudStorageRepository
{
    private readonly StorageClient _storageClient;
    private readonly string _bucketName;

    public GoogleCloudStorageRepository(string bucketName)
    {
        _storageClient = StorageClient.Create();
        _bucketName = bucketName ?? throw new ArgumentNullException(nameof(bucketName));
    }

    public async Task<string> UploadFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            throw new ArgumentException("File is null or empty", nameof(file));
        }

        // Generate a unique blob name with the current timestamp
        var blobName = $"{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}-{Guid.NewGuid()}-{file.FileName}";

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            // Upload file to Google Cloud Storage
            var blob = await _storageClient.UploadObjectAsync(
                _bucketName,
                blobName,
                file.ContentType,
                memoryStream);

            return $"https://storage.googleapis.com/{_bucketName}/{blobName}";
        }
    }
}
