namespace DeforumBeatSync;

public class SectionModel
{
    private readonly int _fps;

    public SectionModel(int fps)
    {
        _fps = fps;
    }
    
    public TimeOnly StartTime { get; init; }
    public SectionType Type { get; init; }
    public string Notes { get; init; }
    
    public TimeOnly EndTime { get; set; }
    public double DurationInSeconds => (EndTime - StartTime).TotalSeconds;
    public int DurationInFrames => (int)Math.Floor(DurationInSeconds * _fps);
}