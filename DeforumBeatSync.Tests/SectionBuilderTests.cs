using DeforumBeatSync.Section;
using Moq;

namespace DeforumBeatSync.Tests;

public class SectionBuilderTests
{
    private readonly Mock<IFileAdapter> _fileAdapterMock = new Mock<IFileAdapter>();

    [Fact]
    public async Task GetSections_ByDefault_ReturnsSections()
    {
        _fileAdapterMock.Setup(x => x.ReadFileAsJson<IEnumerable<SectionSetting>>(It.IsAny<string>()))
            .ReturnsAsync(new List<SectionSetting>
            {
                new()
                {
                    StartTime = new TimeSpan(0, 0, 0)
                },
                new()
                {
                    StartTime = new TimeSpan(0, 0, 15)
                }
            });
        
        var subject = GetSubject();
        var result = await subject.GetSections("sections.json", new SettingsFake
        {
            VideoLength = new TimeSpan(0, 0, 45)
        });

        Assert.Equal(2, result.Count);
        
        Assert.Equal(new TimeOnly(0, 0, 0), result[0].StartTime);
        Assert.Equal(new TimeOnly(0, 0, 15), result[0].EndTime);
        
        Assert.Equal(new TimeOnly(0, 0, 15), result[1].StartTime);
        Assert.Equal(new TimeOnly(0, 0, 45), result[1].EndTime);
    }
    
    private SectionBuilder GetSubject()
    {
        return new SectionBuilder(_fileAdapterMock.Object);
    }
}