namespace DeforumBeatSync;

public interface IBarCounter
{
    Task<IEnumerable<Bar>> GetBars(string file);
}

public class BarCounter : IBarCounter
{
    private readonly IFileReader _fileReader;
    private readonly IFrameParser _frameParser;
    private readonly IBarTypeResolver _barTypeResolver;

    public BarCounter(
        IFileReader fileReader,
        IFrameParser frameParser,
        IBarTypeResolver barTypeResolver)
    {
        _fileReader = fileReader;
        _frameParser = frameParser;
        _barTypeResolver = barTypeResolver;
    }
    
    public async Task<IEnumerable<Bar>> GetBars(string file)
    {
        var fileContents = await _fileReader.ReadFile(file);
        var frames = _frameParser.ReadFrames(fileContents).ToList();

        return DetermineBars(frames);
    }

    private List<Bar> DetermineBars(List<FrameValue> allFrames)
    {
        const int eighthNoteFrameCount = Settings.QuarterNoteFrameCount / 2;
        
        var bars = new List<Bar>();
        var beats = new List<FrameValue>();
        var barNumber = 1;
        
        for (int frameIndex = 0; frameIndex < allFrames.Count; frameIndex += Settings.QuarterNoteFrameCount)
        {
            var minFrameNumber = frameIndex - eighthNoteFrameCount;
            var maxFrameNumber = (frameIndex + Settings.QuarterNoteFrameCount) - eighthNoteFrameCount;
            var quarterNote = allFrames
                .Where(f => f.Frame >= minFrameNumber && f.Frame <= maxFrameNumber)
                .OrderByDescending(f => f.Value)
                .Take(1)
                .Single();
            
            beats.Add(quarterNote);
            
            if (beats.Count != 4) 
                continue;
            
            bars.Add(new Bar
            {
                Number = barNumber,
                Type = _barTypeResolver.GetType(beats),
                Beats = beats.ToList()
            });
            beats.Clear();
            barNumber++;
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