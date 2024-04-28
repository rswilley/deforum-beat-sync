namespace DeforumBeatSync.Tests;

public class SettingsFake : ISettings
{
    public TimeSpan VideoLength { get; init; } = new TimeSpan(0, 3, 15);
    public int Bpm { get; init; } = 128;
    public int Fps { get; init; } = 24;
    public int PromptLengthInSeconds { get; init; } = 30;
    public int BarFrameCount { get; init; } = 48;
    public int HalfNoteFrameCount => BarFrameCount / 2;
    public int QuarterNoteFrameCount => BarFrameCount / 4;

    public StrengthSettings Strength { get; init; } = new StrengthSettings
    {
        High = 0.80,
        Low = 0.25,
        Constant = 0.65
    };

    public double TranslationX { get; init; } = 0;
    public double TranslationY { get; init; } = 0;
    public double TranslationZ { get; init; } = 0.5;
    public double RotationX { get; init; } = 0.04;
    public double RotationY { get; init; } = 0.06;
    public double RotationZ { get; init; } = -0.03;
}