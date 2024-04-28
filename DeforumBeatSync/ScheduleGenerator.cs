using System.Text;

namespace DeforumBeatSync;

public interface IScheduleGenerator
{
    Task<Schedule> GetSchedule(string filePath, IDictionary<int, SectionModel> sections);
}

public class ScheduleGenerator : IScheduleGenerator
{
    private readonly IBarCounter _barCounter;
    private readonly ISettings _settings;

    public ScheduleGenerator(
        IBarCounter barCounter, 
        ISettings settings)
    {
        _barCounter = barCounter;
        _settings = settings;
    }

    public async Task<Schedule> GetSchedule(string filePath, IDictionary<int, SectionModel> sections)
    {
        var schedule = new Schedule(_settings);
        
        var bars = (await _barCounter.GetBars(filePath)).ToList();
        foreach (var bar in bars)
        {
            foreach (var beat in bar.Beats)
            {
                //schedule.HandleStrength(beat);
                schedule.HandleTranslationZ(beat);
                schedule.HandleTranslationY(beat);
            }
        }

        return schedule;
    }
}

public class Schedule
{
    private readonly ISettings _settings;

    public Schedule(ISettings settings)
    {
        _settings = settings;
        _strength.Add(new FrameSetting
        {
            FrameNumber = 0,
            FrameValue = _settings.Strength.High
        });
    }
    
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
                FrameValue = _settings.Strength.Low
            });
            _strength.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber + 1,
                FrameValue = _settings.Strength.High
            });   
        }
        else
        {
            _strength.Add(new FrameSetting
            {
                FrameNumber = beat.FrameSetting.FrameNumber,
                FrameValue = _settings.Strength.High
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

    private readonly List<FrameSetting> _strength = new();

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