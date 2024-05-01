using System.Diagnostics;
using System.Text;
using System.Text.Json;
using DeforumBeatSync;
using DeforumBeatSync.Extensions;
using DeforumBeatSync.Track;

var stopwatch = Stopwatch.StartNew();

var fileAdapter = new FileAdapter();
var settings = await fileAdapter.ReadFileAsJson<Settings>("settings.json");

var frameParser = new FrameParser();
var track = await new TrackLoader(fileAdapter, frameParser).LoadTrackInfo("track2.json", "frames.txt", settings);
var promptResult = new PromptGenerator().GetPrompts(track, settings);

var output = new StringBuilder();
output.AppendLine($"Total Frames: {promptResult.TotalFrames}");
output.AppendLine("");

output.AppendLine("Prompts:");
output.AppendLine(JsonSerializer.Serialize(promptResult.Prompts, new JsonSerializerOptions
{
    WriteIndented = true
}));
output.AppendLine("");

output.AppendLine("Strength Schedule:");
output.AppendLine(promptResult.MovementSchedule.StrengthSchedule.ToSchedule());
output.AppendLine("");

output.AppendLine("Translation Z:");
output.AppendLine(promptResult.MovementSchedule.TranslationZ.ToSchedule());
output.AppendLine("");

output.AppendLine("Rotation X:");
output.AppendLine(promptResult.MovementSchedule.RotationX.ToSchedule());
output.AppendLine("");

output.AppendLine("Rotation Y:");
output.AppendLine(promptResult.MovementSchedule.RotationY.ToSchedule());
output.AppendLine("");

output.AppendLine("Rotation Z:");
output.AppendLine(promptResult.MovementSchedule.RotationZ.ToSchedule());

await fileAdapter.WriteFile("output.txt", output.ToString());

stopwatch.Stop();
Console.WriteLine($"Finished in {stopwatch.ElapsedMilliseconds}ms!");
Console.ReadKey();
