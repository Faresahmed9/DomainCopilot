namespace DomainCopilot.Application.Services;
//Dense Retrieval
public class CosineSimilarityCalculator
{
    public double Calculate(
        IReadOnlyList<float> vectorA,
        IReadOnlyList<float> vectorB)
    {
        if (vectorA.Count != vectorB.Count)
        {
            throw new ArgumentException(
                "Vectors must have the same dimensions.");
        }

        if (vectorA.Count == 0)
        {
            return 0;
        }

        double dotProduct = 0;
        double magnitudeA = 0;
        double magnitudeB = 0;

        for (int i = 0; i < vectorA.Count; i++)
        {
            dotProduct += vectorA[i] * vectorB[i];
            magnitudeA += vectorA[i] * vectorA[i];
            magnitudeB += vectorB[i] * vectorB[i];
        }

        if (magnitudeA == 0 || magnitudeB == 0)
        {
            return 0;
        }

        return dotProduct /
            (Math.Sqrt(magnitudeA) * Math.Sqrt(magnitudeB));
    }
}