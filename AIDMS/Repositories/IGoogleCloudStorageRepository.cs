namespace AIDMS.Repositories
{
    public interface IGoogleCloudStorageRepository
    {
        Task<string> UploadFileAsync(IFormFile file);
    }
}
