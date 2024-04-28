using System.Text.Json;
using System.Text.Json.Serialization;

namespace DeforumBeatSync;

public interface IFileAdapter
{
    public Task<string> ReadFileAsString(string filePath);
    public Task<T> ReadFileAsJson<T>(string filePath);
    Task WriteFile(string filePath, string contents);
}

public class FileAdapter : IFileAdapter
{
    public async Task<string> ReadFileAsString(string filePath)
    {
        var contents = await File.ReadAllTextAsync(filePath);
        return contents;
    }

    public async Task<T> ReadFileAsJson<T>(string filePath)
    {
        await using var stream = new FileStream(filePath, FileMode.Open);
        
        var options = new JsonSerializerOptions();
        options.Converters.Add(new JsonStringEnumConverter());
        var json = JsonSerializer.Deserialize<T>(stream, options);
        return json!;
    }

    public async Task WriteFile(string filePath, string contents)
    {
        await File.WriteAllTextAsync(filePath, contents);
    }
}