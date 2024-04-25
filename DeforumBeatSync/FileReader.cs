namespace DeforumBeatSync;

public interface IFileAdapter
{
    public Task<string> ReadFile(string filePath);
    Task WriteFile(string filePath, string contents);
}

public class FileAdapter : IFileAdapter
{
    public async Task<string> ReadFile(string filePath)
    {
        var contents = await File.ReadAllTextAsync(filePath);
        return contents;
    }

    public async Task WriteFile(string filePath, string contents)
    {
        await File.WriteAllTextAsync(filePath, contents);
    }
}