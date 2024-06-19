namespace AIDMS.Repositories
{
    public interface IPDFManagementRepository
    {
        Task<string> ReadPDFContent(IFormFile file);
    }
}
