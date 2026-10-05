
using DomainCopilot.Application.DTOs;
using DomainCopilot.Application.Interfaces;
using UglyToad.PdfPig;

namespace DomainCopilot.Infrastructure.DocumentProcessing;

public class PdfTextExtractor : IDocumentTextExtractor
{
    public Task<IReadOnlyList<DocumentPage>> ExtractPagesAsync(
        Stream fileStream,
        string contentType)
    {
        if (!contentType.Equals(
                "application/pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException(
                "Only PDF files are supported.");
        }

        using var document = PdfDocument.Open(fileStream);

        var pages = new List<DocumentPage>();

        foreach (var page in document.GetPages())
        {
            var words = page.GetWords()
                .OrderByDescending(word => word.BoundingBox.Bottom)
                .ThenBy(word => word.BoundingBox.Left)
                .ToList();

            var lines = new List<string>();
            var currentLine = new List<string>();
            double? currentY = null;

            foreach (var word in words)
            {
                var wordY = word.BoundingBox.Bottom;

                if (currentY is null ||
                    Math.Abs(wordY - currentY.Value) < 5)
                {
                    currentLine.Add(word.Text);
                }
                else
                {
                    if (currentLine.Count > 0)
                    {
                        lines.Add(
                            string.Join(" ", currentLine));
                    }

                    currentLine.Clear();
                    currentLine.Add(word.Text);
                }

                currentY = wordY;
            }

            if (currentLine.Count > 0)
            {
                lines.Add(
                    string.Join(" ", currentLine));
            }

            pages.Add(
                new DocumentPage
                {
                    PageNumber = page.Number,
                    Content = string.Join(
                        Environment.NewLine,
                        lines)
                });
        }

        return Task.FromResult<IReadOnlyList<DocumentPage>>(pages);
    }
}

