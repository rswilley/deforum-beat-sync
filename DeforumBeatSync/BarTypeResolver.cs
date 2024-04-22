namespace DeforumBeatSync;

public interface IBarTypeResolver
{
    BarType GetType(List<FrameValue> beats);
}

public class BarTypeResolver : IBarTypeResolver
{
    public BarType GetType(List<FrameValue> beats)
    {
        var barType = BarType.UNKNOWN;
        if (beats.Average(beat => beat.Value) >= 0.5)
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
        else if (double.Round(beats.Sum(beat => beat.Value), 0) == 0)
        {
            barType = BarType.BREAKDOWN;
        }
        return barType;
    }

    private static bool IsBreakdown(List<FrameValue> beats)
    {
        double lastBeat = 0;
        int delta = 0;
        for (int b = 0; b < beats.Count; b++)
        {
            var currentBeatValue = beats.ElementAt(b).Value;
            
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

    private bool IsBuildup(List<FrameValue> beats)
    {
        double lastBeat = 0;
        int delta = 0;
        for (int b = 0; b < beats.Count; b++)
        {
            var currentBeatValue = beats.ElementAt(b).Value;
            
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