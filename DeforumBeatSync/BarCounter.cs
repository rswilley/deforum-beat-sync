﻿namespace DeforumBeatSync;

public interface IBarCounter
{
    Task<IEnumerable<Bar>> GetBars(string file);
}

public class BarCounter : IBarCounter
{
    private readonly IFileAdapter _fileAdapter;
    private readonly IFrameParser _frameParser;
    private readonly IBarTypeResolver _barTypeResolver;

    public BarCounter(
        IFileAdapter fileAdapter,
        IFrameParser frameParser,
        IBarTypeResolver barTypeResolver)
    {
        _fileAdapter = fileAdapter;
        _frameParser = frameParser;
        _barTypeResolver = barTypeResolver;
    }
    
    public async Task<IEnumerable<Bar>> GetBars(string file)
    {
        var fileContents = await _fileAdapter.ReadFile(file);
        var frames = _frameParser.ReadFrames(fileContents);

        return DetermineBars(frames);
    }

    private List<Bar> DetermineBars(Dictionary<int, FrameSetting> allFrames)
    {
        const int eighthNoteFrameCount = Settings.QuarterNoteFrameCount / 2;
        
        var bars = new List<Bar>();
        var beats = new List<Beat>();
        var barNumber = 1;
        
        for (int frameIndex = 0; frameIndex < allFrames.Count; frameIndex += Settings.QuarterNoteFrameCount)
        {
            var minFrameNumber = frameIndex - eighthNoteFrameCount;
            var maxFrameNumber = (frameIndex + Settings.QuarterNoteFrameCount) - eighthNoteFrameCount;
            var quarterNoteFrame = allFrames
                .Where(f => f.Key >= minFrameNumber && f.Key <= maxFrameNumber)
                .OrderByDescending(f => f.Value.FrameValue)
                .Take(1)
                .Single();
            
            beats.Add(new Beat
            {
                FrameSetting = quarterNoteFrame.Value
            });
            
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
    public IEnumerable<Beat> Beats { get; set; }
}

public class Beat
{
    public FrameSetting FrameSetting { get; set; }
    
    public bool IsKick()
    {
        return FrameSetting.FrameValue >= 0.2;
    }
}