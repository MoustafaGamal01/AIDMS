using Google.Cloud.Vision.V1;

namespace AIDMS.Repositories
{
    public interface IGoogleCloudVisionRepository
    {
        Task<IReadOnlyList<AnnotateImageResponse>> AnalyzeDocumentAsync(Image image);
        Task<BatchAnnotateImagesResponse> GetResponseAsync(string imagePath, List<Feature> featureList);

        Task<double> CheckSecondaryCertificateValidationAsync(List<Feature> featureList, string imagePath);

        Task<double> CheckBirthDateCertificateValidationAsync(List<Feature> featureList, string imagePath);

        Task<double> CheckNationalIdValidationAsync(List<Feature> featureList, string imagePath);
    }
}
