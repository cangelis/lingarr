using Lingarr.Server.Interfaces.Services;
using Lingarr.Server.Interfaces.Services.Translation;
using Lingarr.Server.Models;

namespace Lingarr.Server.Services.Translation.Base;

public abstract class BaseTranslationService : ITranslationService
{
    protected readonly ISettingService _settings;
    protected readonly ILogger _logger;

    protected BaseTranslationService(ISettingService settings, ILogger logger)
    {
        _settings = settings;
        _logger = logger;
    }

    /// <inheritdoc />
    public abstract string? ModelName { get; }

    /// <inheritdoc />
    public abstract Task<string> TranslateAsync(
        string text,
        string sourceLanguage,
        string targetLanguage,
        List<string>? contextLinesBefore,
        List<string>? contextLinesAfter,
        List<string>? previouslyTranslatedContext,
        CancellationToken cancellationToken);

    /// <inheritdoc />
    public abstract Task<List<SourceLanguage>> GetLanguages();

    /// <inheritdoc />
    public abstract Task<ModelsResponse> GetModels();

    /// <summary>
    /// Extracts the segment content from an AI response that may contain metadata tags.
    /// If no segment tags are found, returns the full response.
    /// </summary>
    /// <param name="response">The AI translation response</param>
    /// <returns>The extracted segment or full response if no segment found</returns>
    protected string ExtractSegmentFromResponse(string response)
    {
        if (string.IsNullOrWhiteSpace(response))
            return response;

        // Look for <segment>...</segment> tags
        var segmentPattern = @"<segment>(.*?)</segment>";
        var match = System.Text.RegularExpressions.Regex.Match(
            response,
            segmentPattern,
            System.Text.RegularExpressions.RegexOptions.Singleline
        );

        if (match.Success && match.Groups.Count > 1)
        {
            return match.Groups[1].Value.Trim();
        }

        // If no segment tags found, return the full response
        return response;
    }
}