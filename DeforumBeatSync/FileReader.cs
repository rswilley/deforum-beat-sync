namespace DeforumBeatSync;

public interface IFileReader
{
    public Task<string> ReadFile(string file);
}

public class FileReader : IFileReader
{
    public async Task<string> ReadFile(string file)
    {
        var contents = await File.ReadAllTextAsync(file);
        return contents;
    }
}