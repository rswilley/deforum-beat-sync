namespace DeforumBeatSync;

public class Settings
{
    public required int BarFrameCount { get; set; }
    public int HalfNoteBarFrameCount => BarFrameCount / 2;
    public int QuarterNoteBarFrameCount => BarFrameCount / 4;
    public double StrengthHigh { get; set; }
    public double StrengthLow { get; set; }
}