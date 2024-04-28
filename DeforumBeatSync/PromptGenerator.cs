using System.Text;

namespace DeforumBeatSync;

public interface IPromptGenerator
{
    PromptGeneratorResult GetPrompts(Dictionary<int, SectionModel> sections, ISettings settings);
}

public class PromptGenerator : IPromptGenerator
{
    public PromptGeneratorResult GetPrompts(Dictionary<int, SectionModel> sections, ISettings settings)
    {
        var totalSeconds = settings.VideoLength.TotalSeconds;
        var totalFrames = totalSeconds * settings.Fps;
        
        var prompts = new Dictionary<string, string>();
        var movementSchedule = new MovementSchedule();

        for (int sectionIndex = 0; sectionIndex < sections.Count; sectionIndex++)
        {
            var currentSection = sections[sectionIndex];
            var currentSectionFrame = GetFrameFromTime(currentSection.StartTime, settings.Fps);

            HandleSectionType(currentSection, movementSchedule, currentSectionFrame, settings);
            
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

    private static void HandleSectionType(SectionModel currentSection, MovementSchedule movementSchedule, int frameStart, ISettings settings)
    {
        switch (currentSection.Type)
        {
            case SectionType.INTRO:
            case SectionType.OUTRO:
            case SectionType.CHORUS:
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
        var totalSeconds = minutesAsSeconds + currentSectionStartTime.Second;

        return totalSeconds * fps;
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
}