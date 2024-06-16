using Google.Cloud.Vision.V1;

namespace AIDMS.Repositories
{
    public interface IGoogleCloudVisionService
    {
        Task<IReadOnlyList<AnnotateImageResponse>> AnalyzeDocumentAsync(Image image);

    }
}
