//using DomainCopilot.Application.Interfaces;
//using OpenAI.Embeddings;

//namespace DomainCopilot.Infrastructure.Embeddings;

//public class OpenAIEmbeddingService : IEmbeddingService
//{
//    private readonly EmbeddingClient _client;

//    public OpenAIEmbeddingService(string apiKey)
//    {
//        _client = new EmbeddingClient(
//            "text-embedding-3-small",
//            apiKey);
//    }

//    public async Task<IReadOnlyList<float>> GenerateEmbeddingAsync(
//        string text)
//    {
//        var result = await _client.GenerateEmbeddingAsync(text);

//        return result.Value.ToFloats().ToArray();
//    }
//}