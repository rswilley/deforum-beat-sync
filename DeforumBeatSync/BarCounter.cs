namespace DeforumBeatSync;

public interface IBarCounter
{
    Task<IEnumerable<Bar>> GetBars(string file, Settings settings);
}

public class BarCounter : IBarCounter
{
    private readonly IFileReader _fileReader;
    private readonly IFrameParser _frameParser;

    public BarCounter(
        IFileReader fileReader,
        IFrameParser frameParser)
    {
        _fileReader = fileReader;
        _frameParser = frameParser;
    }
    
    public async Task<IEnumerable<Bar>> GetBars(string file, Settings settings)
    {
        var fileContents = await _fileReader.ReadFile(file);
        var frames = _frameParser.ReadFrames(fileContents).ToList();
        var bars = new List<Bar>();

        var totalBarCount = frames.Count / settings.BarFrameCount;
        var barStart = 0;
        var barEnd = settings.BarFrameCount - 1;

        for (int b = 0; b < totalBarCount; b++)
        {
            if (b > 0)
            {
                barStart += settings.BarFrameCount;
                barEnd = barStart + settings.BarFrameCount - 1;
            }

            var frameStart = barStart;
            var frameEnd = barEnd;
            var topBeats = frames
                .Where(f => f.Frame >= frameStart && f.Frame <= frameEnd)
                .OrderByDescending(f => f.Value)
                .Take(4)
                .OrderBy(bar => bar.Frame)
                .ToList();

            var barType = BarType.UNKNOWN;
            if (double.Round(topBeats.Sum(beat => beat.Value), 0) == 0)
            {
                barType = BarType.BREAKDOWN;
            } else if (topBeats.Average(beat => beat.Value) >= 0.5)
            {
                barType = BarType.CHORUS;
            }

            bars.Add(new Bar
            {
                Number = b + 1,
                Type = barType,
                Beats = topBeats
            });
        }

        return bars;
    }
}

public class Bar
{
    public int Number { get; set; }
    public BarType Type { get; set; }
    public IEnumerable<FrameValue> Beats { get; set; }
}