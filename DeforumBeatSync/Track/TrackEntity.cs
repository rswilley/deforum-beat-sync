using System.Text.Json.Serialization;
using DeforumBeatSync.Section;

namespace DeforumBeatSync.Track;

public interface ITrack
{
    TimeSpan TrackLength { get; init; }
    public int Bpm { get; init; }
}

public class TrackEntity : ITrack
{
    [JsonPropertyName("trackLength")]
    public TimeSpan TrackLength { get; init; }
    [JsonPropertyName("bpm")]
    public int Bpm { get; init; }
    [JsonPropertyName("sections")]
    public List<SectionEntity> Sections { get; init; }
}

public class TrackModel : ITrack
{
    public TimeSpan TrackLength { get; init; }
    public int Bpm { get; init; }
    public Dictionary<int, SectionModel> Sections { get; init; }
}