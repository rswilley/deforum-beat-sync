namespace DeforumBeatSync;

public interface IFrameParser
{
    IEnumerable<FrameValue> ReadFrames(string frameFileContents);
}

public class FrameParser : IFrameParser
{
    public IEnumerable<FrameValue> ReadFrames(string frameFileContents)
    {
        var frames = frameFileContents.Split(",");

        return frames
            .Select(frame => frame.Trim())
            .Select(trimmed => trimmed.Split(":"))
            .Select(values => new FrameValue
            {
                Frame = Convert.ToInt32(values[0]),
                Value = Convert.ToDouble(values[1].Replace("(", "").Replace(")", ""))
            });
    }
}

public class FrameValue
{
    public int Frame { get; set; }
    public double Value { get; set; }
}