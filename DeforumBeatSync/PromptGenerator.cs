using System.Text;
using DeforumBeatSync.Bars;
using DeforumBeatSync.Section;
using DeforumBeatSync.Track;

namespace DeforumBeatSync;

public interface IPromptGenerator
{
    PromptGeneratorResult GetPrompts(TrackModel track, ISettings settings);
}

public class PromptGenerator : IPromptGenerator
{
    public PromptGeneratorResult GetPrompts(TrackModel track, ISettings settings)
    {
        var totalSeconds = track.TrackLength.TotalSeconds;
        var totalFrames = totalSeconds * settings.Fps;
        
        var prompts = new Dictionary<string, string>();
        var movementSchedule = new MovementSchedule();

        for (int sectionIndex = 0; sectionIndex < track.Sections.Count; sectionIndex++)
        {
            var currentSection = track.Sections[sectionIndex];
            var currentSectionFrame = GetFrameFromTime(currentSection.StartTime, settings.Fps);

            HandleSectionType(currentSection, movementSchedule, currentSectionFrame, track.Frames, settings);
            
            prompts.Add(currentSectionFrame.ToString(), GetPromptText(currentSection, false));
            
            if (currentSection.DurationInSeconds > settings.PromptLengthInSeconds)
            {
                var remainingSeconds = currentSection.DurationInSeconds - settings.PromptLengthInSeconds;
                var frameIteration = currentSectionFrame;
                while (remainingSeconds > 0)
                {
                    var subFrameIndex = frameIteration +
                                        GetFrameFromTime(new TimeOnly(0, 0, settings.PromptLengthInSeconds), settings.Fps);
                    prompts.Add(subFrameIndex.ToString(), GetPromptText(currentSection, true));
                    remainingSeconds -= settings.PromptLengthInSeconds;
                    frameIteration = subFrameIndex;
                }
            }
        }

        return new PromptGeneratorResult
        {
            TotalFrames = (int)totalFrames + settings.Fps,
            Prompts = prompts,
            MovementSchedule = movementSchedule
        };
    }

    private static void HandleSectionType(SectionModel currentSection, MovementSchedule movementSchedule, int frameStart, Dictionary<int, FrameSetting> frames, ISettings settings)
    {
        switch (currentSection.Type)
        {
            case SectionType.INTRO:
            case SectionType.OUTRO:
                movementSchedule.TranslationZ.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.TranslationZ
                });
                movementSchedule.RotationX.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.RotationX
                });
                movementSchedule.RotationY.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.RotationY
                });
                movementSchedule.RotationZ.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.RotationZ
                });
                movementSchedule.StrengthSchedule.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.Strength.Constant
                });
                break;
            case SectionType.BUILDUP:
                movementSchedule.TranslationZ.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.TranslationZ
                });
                movementSchedule.RotationX.AddRange(GetBuildupRotationX(currentSection, frameStart, settings));
                movementSchedule.RotationY.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.RotationY
                });
                movementSchedule.RotationZ.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.RotationZ
                });
                movementSchedule.StrengthSchedule.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.Strength.Constant
                });
                break;
            case SectionType.BREAKDOWN:
                movementSchedule.TranslationZ.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = 0
                });
                movementSchedule.RotationX.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = 0
                });
                movementSchedule.RotationY.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = 0
                });
                movementSchedule.RotationZ.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = 0
                });
                movementSchedule.StrengthSchedule.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.Strength.Constant
                });
                break;
            case SectionType.CHORUS:
                movementSchedule.TranslationZ.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = 0,
                    UseFormula = true,
                    Formula = "(-(2 * 1 / 3.141) * arctan((1 * 1 + 1) / tan(((t  + 0) * 3.141 * 128 / 60 / 24))) + 1.50)" // TODO: remove hardcoded
                });
                movementSchedule.RotationX.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.RotationX
                });
                movementSchedule.RotationY.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.RotationY
                });
                movementSchedule.RotationZ.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.RotationZ
                });
                movementSchedule.StrengthSchedule.Add(new FrameSetting
                {
                    FrameNumber = frameStart,
                    FrameValue = settings.Strength.Constant
                });
                break;
        }
    }

    /// <summary>
    /// Calculate a sawtooth wave for buildup
    /// </summary>
    /// <param name="currentSection"></param>
    /// <param name="frameStart"></param>
    /// <param name="settings"></param>
    /// <returns></returns>
    private static IEnumerable<FrameSetting> GetBuildupRotationX(SectionModel currentSection, int frameStart, ISettings settings)
    {
        var results = new List<FrameSetting>();
        for (var f = 0; f < currentSection.DurationInFrames; f++) {
            var frameNumber = frameStart + f;
            var value = Sawtooth(f + 1, currentSection.DurationInFrames, settings.RotationX, 1);
            var roundedValue = Math.Round(value / 2, 2);

            results.Add(new FrameSetting
            {
                FrameNumber = frameNumber,
                FrameValue = roundedValue
            });
        }

        return results;

        static double Sawtooth(double t, double period, double min, double max)
        {
            return min + (max - min) * Fract(t / period);
        }

        static double Fract(double t)
        {
            return t % 1.0;
        }
    }

    private static int GetFrameFromTime(TimeOnly currentSectionStartTime, int fps)
    {
        var minutesAsSeconds = currentSectionStartTime.Minute * 60;
        var totalSeconds = minutesAsSeconds + new TimeSpan(0, 0, 0, currentSectionStartTime.Second, currentSectionStartTime.Millisecond).TotalSeconds;
        var frame = (int)Math.Floor(totalSeconds * fps);
        return frame;
    }

    private static string GetPromptText(SectionModel section, bool isSubPrompt)
    {
        var sb = new StringBuilder();
        sb.Append(section.Type.ToString());

        if (isSubPrompt)
        {
            sb.Append(" - " + "sub prompt");
        }

        if (!string.IsNullOrEmpty(section.Notes))
        {
            sb.Append($" [{section.Notes}]");
        }

        return sb.ToString();
    }
}

public class PromptGeneratorResult
{
    public int TotalFrames { get; init; }
    public Dictionary<string, string> Prompts { get; init; }
    public MovementSchedule MovementSchedule { get; init; }
}

public class MovementSchedule
{
    public List<FrameSetting> TranslationZ = new();
    public List<FrameSetting> RotationX = new();
    public List<FrameSetting> RotationY = new();
    public List<FrameSetting> RotationZ = new();
    public List<FrameSetting> StrengthSchedule = new();
}