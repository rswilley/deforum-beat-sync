using System.Text;
using DeforumBeatSync;

var fileReader = new FileAdapter();
var frameParser = new FrameParser();
var barTypeResolver = new BarTypeResolver();
var barCounter = new BarCounter(fileReader, frameParser, barTypeResolver);

var scheduleGenerator = new ScheduleGenerator(barCounter);
var schedule = await scheduleGenerator.GetSchedule("frames.txt");

var output = new StringBuilder();
output.AppendLine($"Strength: {schedule.StrengthSchedule}");
output.AppendLine("");
output.AppendLine($"Translation Z: {schedule.TranslationZSchedule}");
output.AppendLine("");
output.AppendLine($"Translation Y: {schedule.TranslationYSchedule}");
output.AppendLine("");

await fileReader.WriteFile("output.txt", output.ToString());

Console.ReadKey();