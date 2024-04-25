namespace DeforumBeatSync;

public interface IFrameParser
{
    Dictionary<int, FrameSetting> ReadFrames(string frameFileContents);
}

public class FrameParser : IFrameParser
{
    public Dictionary<int, FrameSetting> ReadFrames(string frameFileContents)
    {
        var frames = frameFileContents.Split(",");

        return frames
            .Select(frame => frame.Trim())
            .Select(trimmed => trimmed.Split(":"))
            .ToDictionary(values => Convert.ToInt32(values[0]), values => new FrameSetting
            {
                FrameNumber = Convert.ToInt32(values[0]),
                FrameValue = Convert.ToDouble(values[1].Replace("(", "").Replace(")", ""))
            });
    }
}

public class FrameSetting
{
    public int FrameNumber { get; init; }
    public double FrameValue { get; init; }
}