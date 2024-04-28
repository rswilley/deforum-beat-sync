using System.Text;
using System.Text.Json;
using DeforumBeatSync;
using DeforumBeatSync.Section;

var fileAdapter = new FileAdapter();
var settings = await fileAdapter.ReadFileAsJson<Settings>("settings.json");

var sections = await new SectionBuilder(fileAdapter).GetSections("sections.json", settings);

var frameParser = new FrameParser();
var barTypeResolver = new BarTypeResolver();

var barCounter = new BarCounter(fileAdapter, frameParser, barTypeResolver, settings);
var promptResult = new PromptGenerator().GetPrompts(sections, settings);

var scheduleGenerator = new ScheduleGenerator(barCounter, settings);
var schedule = await scheduleGenerator.GetSchedule("frames.txt", sections);

var output = new StringBuilder();
output.AppendLine($"Total Frames: {promptResult.TotalFrames}");
output.AppendLine("");
output.AppendLine("Prompts:");
output.AppendLine(JsonSerializer.Serialize(promptResult.Prompts, new JsonSerializerOptions
{
    WriteIndented = true
}));
output.AppendLine("");
output.AppendLine($"Strength: {schedule.StrengthSchedule}");
output.AppendLine("");
output.AppendLine($"Translation Z: {schedule.TranslationZSchedule}");
output.AppendLine("");
output.AppendLine($"Translation Y: {schedule.TranslationYSchedule}");
output.AppendLine("");

await fileAdapter.WriteFile("output.txt", output.ToString());
