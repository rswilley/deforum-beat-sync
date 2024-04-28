using System.Text.Json.Serialization;

namespace DeforumBeatSync.Section;

public class SectionSetting
{
    [JsonPropertyName("startTime")]
    public TimeSpan StartTime { get; init; }
    [JsonPropertyName("type")]
    public SectionType Type { get; init; }
    [JsonPropertyName("notes")]
    public string Notes { get; init; }
}