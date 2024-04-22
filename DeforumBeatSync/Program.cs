// See https://aka.ms/new-console-template for more information

using System.Text;
using DeforumBeatSync;

// TODO:
// Handle strength schedule, zoom on beat, rotation X on build-up, handle breakdowns, etc

var fileReader = new FileReader();
var frameParser = new FrameParser();
var barTypeResolver = new BarTypeResolver();
var barCounter = new BarCounter(fileReader, frameParser, barTypeResolver);
var bars = await barCounter.GetBars("frames.txt");
var beats = bars.SelectMany(bar => bar.Beats);

var strengthSchedule = new List<FrameValue>
{
    new()
    {
        Frame = 0,
        Value = Settings.StrengthHigh
    }
};

var translationZSchedule = new List<FrameValue>()
{
    new()
    {
        Frame = 0,
        Value = 0
    }
};

var translationYSchedule = new List<FrameValue>()
{
    new()
    {
        Frame = 0,
        Value = 0
    }
};

foreach (var beat in beats)
{
    var currentBeatValue = beat.Value;
    if (IsKick(currentBeatValue))
    {
        strengthSchedule.Add(new FrameValue
        {
            Frame = beat.Frame,
            Value = Settings.StrengthLow
        });
        strengthSchedule.Add(new FrameValue
        {
            Frame = beat.Frame + 1,
            Value = Settings.StrengthHigh
        });
        translationZSchedule.Add(new FrameValue
        {
            Frame = beat.Frame,
            Value = 3
        });
        translationZSchedule.Add(new FrameValue
        {
            Frame = beat.Frame + 1,
            Value = 0
        });
        translationYSchedule.Add(new FrameValue
        {
            Frame = beat.Frame,
            Value = 3
        });
        translationYSchedule.Add(new FrameValue
        {
            Frame = beat.Frame + 1,
            Value = 0
        });
    }
    else
    {
        strengthSchedule.Add(new FrameValue
        {
            Frame = beat.Frame,
            Value = Settings.StrengthHigh
        });
        translationZSchedule.Add(new FrameValue
        {
            Frame = beat.Frame,
            Value = 0
        });
        translationYSchedule.Add(new FrameValue
        {
            Frame = beat.Frame,
            Value = 0
        });
    }
}

Console.WriteLine($"Strength Schedule: {GetScheduleAsString(strengthSchedule)}");
Console.WriteLine("");
Console.WriteLine($"Translation Y: {GetScheduleAsString(translationYSchedule)}");
Console.WriteLine("");
Console.WriteLine($"Translation Z: {GetScheduleAsString(translationZSchedule)}");
Console.ReadKey();

bool IsKick(double beatValue)
{
    return beatValue >= 0.2;
}

string GetScheduleAsString(IEnumerable<FrameValue> items)
{
    var sb = new StringBuilder();
    foreach (var item in items)
    {
        sb.Append($"{item.Frame}: ({item.Value}), ");
    }
    return sb.ToString().TrimEnd(',').TrimEnd(' ');
}