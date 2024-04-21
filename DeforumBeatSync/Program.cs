// See https://aka.ms/new-console-template for more information

var settings = new Settings
{
    BPM = 128,
    FPS = 24,
    Cadence = 2,
    StrengthHigh = .8,
    StrengthLow = .35
};

var intro = new Section(SectionType.INTRO, new TimeOnly(0, 0, 0), new TimeOnly(0, 0, 15), settings);
var buildup = new Section(SectionType.BUILDUP, intro.End, new TimeOnly(0, 0, 30), settings);
// var chorus = new Section(SectionType.CHORUS, buildup.End, new TimeOnly(0, 0, 45));
// var breakdown = new Section(SectionType.BREAKDOWN, chorus.End, new TimeOnly(0, 1, 45));
// var outro = new Section(SectionType.OUTRO, breakdown.End, new TimeOnly(0, 2, 45));

var tracks = new List<Track>
{
    new()
    {
        Sections = new List<Section>
        {
            intro,
            buildup,
            // chorus,
            // outro
        }
    }
};

foreach (var track in tracks)
{
    var strengthScheduleList = new List<StrengthSchedule>();
    var strengthScheduleUniqueList = new List<StrengthSchedule>();
    var frameCount = 0;
    
    foreach (var section in track.Sections)
    {
        var quarterNote = settings.FPS / settings.BeatsPerSecond;
        var quarterNoteFrame = GetQuarterNoteFrame(quarterNote);
        
        for (int f = 0; f < section.DurationInFrames; f++)
        {
            var strengthValue = settings.StrengthHigh;
            if (f == quarterNoteFrame)
            {
                strengthValue = settings.StrengthLow;
                quarterNoteFrame += GetQuarterNoteFrame(quarterNote);
            }
            strengthScheduleList.Add(new StrengthSchedule
            {
                Frame = frameCount,
                Value = strengthValue
            });
            frameCount++;
        }
    }
    
    StrengthSchedule? previousSchedule = null;
    foreach (var schedule in strengthScheduleList)
    {
        if (previousSchedule == null)
        {
            strengthScheduleUniqueList.Add(schedule);
        }
        else if (previousSchedule.Value.ToString() != schedule.Value.ToString())
        {
            strengthScheduleUniqueList.Add(schedule);
        }
            
        previousSchedule = schedule;
    }
    
    Console.WriteLine("Strength Schedule: " + ConvertToStrengthSetting(strengthScheduleUniqueList));
        
    var totalFrames = tracks.SelectMany(t => t.Sections).Sum(s => s.DurationInFrames);
    Console.WriteLine("Total Frames: " + totalFrames);
}

Console.WriteLine("Done!");

int GetQuarterNoteFrame(double quarterNote)
{
    return (int)Math.Round(quarterNote, 0);
}

string ConvertToStrengthSetting(List<StrengthSchedule> schedules)
{
    var result = schedules.Select(schedule => $"{schedule.Frame}: ({schedule.Value})").ToList();
    return string.Join(", ", result);
}

class Settings
{
    public int BPM { get; set; }
    public int FPS { get; set; }
    public int Cadence { get; set; }
    public double StrengthHigh { get; set; }
    public double StrengthLow { get; set; }
    public double BeatsPerSecond => (double)BPM / 60;
}

class Track
{
    public IEnumerable<Section> Sections { get; set; }
}

class Section
{
    private readonly SectionType _type;
    private readonly Settings _settings;

    public Section(SectionType type, TimeOnly start, TimeOnly end, Settings settings)
    {
        _type = type;
        _settings = settings;
        Start = start;
        End = end;
    }

    public TimeOnly Start { get; }
    public TimeOnly End { get; }
    public double DurationInSeconds => (End - Start).TotalSeconds;
    public int DurationInFrames => (int)Math.Floor(DurationInSeconds * _settings.FPS + _settings.Cadence);
}

enum SectionType
{
    INTRO,
    BUILDUP,
    BREAKDOWN,
    CHORUS,
    OUTRO
}

class StrengthSchedule
{
    public int Frame { get; set; }
    public double Value { get; set; }
}