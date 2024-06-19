using Google.Cloud.Vision.V1;

namespace AIDMS.Repositories
{
    public interface IGoogleCloudVisionRepository
    {
        Task<IReadOnlyList<AnnotateImageResponse>> AnalyzeDocumentAsync(Image image);
        Task<BatchAnnotateImagesResponse> GetResponseAsync(string imagePath, List<Feature> featureList);

        Task<double> CheckSecondaryCertificateValidationAsync(string imagePath);

        Task<double> CheckBirthDateCertificateValidationAsync(string imagePath);

        Task<double> CheckNationalIdValidationAsync(string imagePath);
    }
}
