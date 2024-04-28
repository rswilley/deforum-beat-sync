namespace DeforumBeatSync.Bars;

public interface IBarTypeResolver
{
    BarType GetType(List<Beat> beats);
}

public class BarTypeResolver : IBarTypeResolver
{
    public BarType GetType(List<Beat> beats)
    {
        var barType = BarType.UNKNOWN;
        if (beats.Average(beat => beat.FrameSetting.FrameValue) >= 0.5)
        {
            barType = BarType.CHORUS;
        }
        else if (IsBreakdown(beats))
        {
            barType = BarType.BREAKDOWN;
        }
        else if (IsBuildup(beats))
        {
            barType = BarType.BUILDUP;
        }
        else if (double.Round(beats.Sum(beat => beat.FrameSetting.FrameValue), 0) == 0)
        {
            barType = BarType.BREAKDOWN;
        }
        return barType;
    }

    private static bool IsBreakdown(List<Beat> beats)
    {
        double lastBeat = 0;
        int delta = 0;
        for (int b = 0; b < beats.Count; b++)
        {
            var currentBeatValue = beats.ElementAt(b).FrameSetting.FrameValue;
            
            if (b == 0)
            {
                lastBeat = currentBeatValue;
                continue;
            }

            if (currentBeatValue < lastBeat)
            {
                delta--;
            }
        }

        return delta <= -3;
    }

    private bool IsBuildup(List<Beat> beats)
    {
        double lastBeat = 0;
        int delta = 0;
        for (int b = 0; b < beats.Count; b++)
        {
            var currentBeatValue = beats.ElementAt(b).FrameSetting.FrameValue;
            
            if (b == 0)
            {
                lastBeat = currentBeatValue;
                continue;
            }

            if (currentBeatValue > lastBeat)
            {
                delta++;
            }
        }

        return delta >= 3;
    }
}