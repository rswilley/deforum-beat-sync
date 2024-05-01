using DeforumBeatSync.Section;
using DeforumBeatSync.Track;
using Moq;

namespace DeforumBeatSync.Tests;

public class TrackLoaderTests
{
    private readonly Mock<IFileAdapter> _fileAdapterMock = new();
    private readonly Mock<IFrameParser> _frameParserMock = new();

    [Fact]
    public async Task LoadTrackInfo_ByDefault_ReturnsSections()
    {
        _fileAdapterMock.Setup(x => x.ReadFileAsJson<TrackEntity>(It.IsAny<string>()))
            .ReturnsAsync(new TrackEntity
            {
                TrackLength = new TimeSpan(0, 0, 45),
                Sections = new List<SectionEntity>
                {
                    new()
                    {
                        StartTime = new TimeSpan(0, 0, 0)
                    },
                    new()
                    {
                        StartTime = new TimeSpan(0, 0, 15)
                    }
                }
            });
        
        var subject = GetSubject();
        var track = await subject.LoadTrackInfo("sections.json", "frames.txt", new SettingsFake());

        Assert.Equal(2, track.Sections.Count);
        
        Assert.Equal(new TimeOnly(0, 0, 0), track.Sections[0].StartTime);
        Assert.Equal(new TimeOnly(0, 0, 15), track.Sections[0].EndTime);
        
        Assert.Equal(new TimeOnly(0, 0, 15), track.Sections[1].StartTime);
        Assert.Equal(new TimeOnly(0, 0, 45), track.Sections[1].EndTime);
    }
    
    private TrackLoader GetSubject()
    {
        return new TrackLoader(_fileAdapterMock.Object, _frameParserMock.Object);
    }
}