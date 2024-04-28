namespace DeforumBeatSync.Section;

public interface ISectionBuilder
{
    Task<Dictionary<int, SectionModel>> GetSections(string filePath, ISettings settings);
}

public class SectionBuilder : ISectionBuilder
{
    private readonly IFileAdapter _fileAdapter;

    public SectionBuilder(IFileAdapter fileAdapter)
    {
        _fileAdapter = fileAdapter;
    }
    
    public async Task<Dictionary<int, SectionModel>> GetSections(string filePath, ISettings settings)
    {
        var sections =
            (await _fileAdapter.ReadFileAsJson<IEnumerable<SectionSetting>>(filePath)).Select(m =>
                new SectionModel(settings.Fps)
                {
                    StartTime = new TimeOnly(m.StartTime.Ticks),
                    Type = m.Type,
                    Notes = m.Notes
                }).ToList();

        var result = new Dictionary<int, SectionModel>();
        double sectionsSumDurationSeconds = 0;
        for (int i = 0; i < sections.Count; i++)
        {
            var currentSection = sections[i];
            var nextIndex = i + 1;
            if (nextIndex < sections.Count)
            {
                var nextSection = sections[nextIndex];
                currentSection.EndTime = nextSection.StartTime;
                sectionsSumDurationSeconds += (currentSection.EndTime - currentSection.StartTime).TotalSeconds;
            }
            else
            {
                var remainingSeconds = settings.VideoLength.TotalSeconds - sectionsSumDurationSeconds;
                var endTimeTs = new TimeSpan(currentSection.StartTime.Ticks) + new TimeSpan(0, 0, (int)remainingSeconds);
                currentSection.EndTime = new TimeOnly(endTimeTs.Ticks);
            }

            result.Add(i, currentSection);
        }
        return result;
    }
}