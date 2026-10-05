using System.Text.RegularExpressions;
using DomainCopilot.Application.Interfaces;

namespace DomainCopilot.Infrastructure.DocumentProcessing;

public class TextCleaner : ITextCleaner
{
    public string Clean(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var cleanedText = text
            .Replace("\r\n", "\n")
            .Replace("\r", "\n");

        cleanedText = Regex.Replace(
            cleanedText,
            @"[ \t]+",
            " ");

        cleanedText = Regex.Replace(
            cleanedText,
            @"\n{3,}",
            "\n\n");

        return cleanedText.Trim();
    }
}