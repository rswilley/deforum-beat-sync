using System.Text;

namespace DeforumBeatSync;

public interface IScheduleGenerator
{
    Task<Schedule> GetSchedule(string filePath);
}

public class ScheduleGenerator : IScheduleGenerator
{
    private readonly IBarCounter _barCounter;

    public ScheduleGenerator(IBarCounter barCounter)
    {
        _barCounter = barCounter;
    }

    public async Task<Schedule> GetSchedule(string filePath)
    {
        var schedule = new Schedule();
        
        var bars = (await _barCounter.GetBars(filePath)).ToList();
        for (int i = 0; i < bars.Count; i++)
        {
            var nextIndex = i + 1;
            if (nextIndex < bars.Count)
            {
                //TODO
                var nextBar = bars[nextIndex];   
            }
            foreach (var beat in bars[i].Beats)
            {
                schedule.HandleStrength(beat);
                schedule.HandleTranslationZ(beat);
                schedule.HandleTranslationY(beat);
            } 
        }

        return schedule;
    }
}

public class Schedule
{
    public string StrengthSchedule => GetScheduleAsString(_strength);
    public string TranslationZSchedule => GetScheduleAsString(_translationZSchedule);
    public string TranslationYSchedule => GetScheduleAsString(_translationYSchedule);

    public void HandleStrength(Beat beat)
    {
        if (beat.IsKick())
        {
            _strength.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber,
                FrameValue = Settings.StrengthLow
            });
            _strength.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber + 1,
                FrameValue = Settings.StrengthHigh
            });   
        }
        else
        {
            _strength.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber,
                FrameValue = Settings.StrengthHigh
            });
        }
    }

    public void HandleTranslationZ(Beat beat)
    {
        if (beat.IsKick())
        {
            _translationZSchedule.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber,
                FrameValue = 3
            });
            _translationZSchedule.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber + 1,
                FrameValue = 0
            });
        }
        else
        {
            _translationZSchedule.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber,
                FrameValue = 0
            });
        }
    }

    public void HandleTranslationY(Beat beat)
    {
        if (beat.IsKick())
        {
            _translationYSchedule.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber,
                FrameValue = 3
            });
            _translationYSchedule.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber + 1,
                FrameValue = 0
            });
        }
        else
        {
            _translationYSchedule.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber,
                FrameValue = 0
            });
        }
    }
    
    private readonly List<FrameSetting> _strength = new()
    {
        new FrameSetting
        {
            FrameNumber = 0,
            FrameValue = Settings.StrengthHigh
        }
    };
    
    private readonly List<FrameSetting> _translationZSchedule = new()
    {
        new FrameSetting
        {
            FrameNumber = 0,
            FrameValue = 0
        }
    };
        
    private readonly List<FrameSetting> _translationYSchedule = new()
    {
        new FrameSetting
        {
            FrameNumber = 0,
            FrameValue = 0
        }
    };
    
    private string GetScheduleAsString(IEnumerable<FrameSetting> items)
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            sb.Append($"{item.FrameNumber}: ({item.FrameValue}), ");
        }
        return sb.ToString().TrimEnd(',').TrimEnd(' ');
    }
}