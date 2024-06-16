using AIDMS.Entities;
using AIDMS.Repositories;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Vision.V1;
using Grpc.Auth;
using Microsoft.Extensions.Options;

public class GoogleCloudVisionService : IGoogleCloudVisionService
{
    private readonly ImageAnnotatorClient _client;

    public GoogleCloudVisionService(IOptions<GoogleCloudVisionOptions> options)
    {
        var credential = GoogleCredential.FromFile(options.Value.ServiceAccountKeyPath)
            .CreateScoped(ImageAnnotatorClient.DefaultScopes);
        _client = new ImageAnnotatorClientBuilder
        {
            ChannelCredentials = credential.ToChannelCredentials()
        }.Build();
    }

    public async Task<IReadOnlyList<AnnotateImageResponse>> AnalyzeDocumentAsync(Image image)
    {
        var features = new List<Feature>
        {
            new Feature { Type = Feature.Types.Type.TextDetection },
            //new Feature { Type = Feature.Types.Type.LabelDetection },
            //new Feature { Type = Feature.Types.Type.ImageProperties },
            //new Feature { Type = Feature.Types.Type.SafeSearchDetection },
            //new Feature { Type = Feature.Types.Type.WebDetection }
        };

        var requests = new List<AnnotateImageRequest>
        {
            new AnnotateImageRequest
            {
                Image = image,
                Features = { features }
            }
        };

        var response = await _client.BatchAnnotateImagesAsync(requests);
        return response.Responses;
    }
}
