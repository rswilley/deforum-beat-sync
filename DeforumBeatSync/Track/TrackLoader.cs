namespace DeforumBeatSync.Track;

public interface ITrackLoader
{
    Task<TrackModel> LoadTrackInfo(string filePath, ISettings settings);
}

public class TrackLoader : ITrackLoader
{
    private readonly IFileAdapter _fileAdapter;

    public TrackLoader(IFileAdapter fileAdapter)
    {
        _fileAdapter = fileAdapter;
    }
    
    public async Task<TrackModel> LoadTrackInfo(string filePath, ISettings settings)
    {
        var track = await _fileAdapter.ReadFileAsJson<TrackEntity>(filePath);
        var sections = track.Sections.Select(m =>
            new SectionModel(settings.Fps)
            {
                StartTime = new TimeOnly(m.StartTime.Ticks),
                Type = m.Type,
                Notes = m.Notes
            }).ToList();

        var sectionDictionary = new Dictionary<int, SectionModel>();
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
                var remainingSeconds = track.TrackLength.TotalSeconds - sectionsSumDurationSeconds;
                var endTimeTs = new TimeSpan(currentSection.StartTime.Ticks) + new TimeSpan(0, 0, (int)remainingSeconds);
                currentSection.EndTime = new TimeOnly(endTimeTs.Ticks);
            }

            sectionDictionary.Add(i, currentSection);
        }
        
        return new TrackModel
        {
            TrackLength = track.TrackLength,
            Bpm = track.Bpm,
            Sections = sectionDictionary
        };
    }
}