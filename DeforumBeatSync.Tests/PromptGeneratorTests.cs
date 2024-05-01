using DeforumBeatSync.Bars;
using DeforumBeatSync.Section;
using DeforumBeatSync.Track;

namespace DeforumBeatSync.Tests;

public class PromptGeneratorTests
{
    [Fact]
    public void GetPrompt_SectionIsLengthOfPromptLength_ReturnsOnePrompt()
    {
        var sections = new Dictionary<int, SectionModel>
        {
            {
                0, new SectionModel(24)
                {
                    StartTime = new TimeOnly(0, 0, 0),
                    EndTime = new TimeOnly(0, 0, 15),
                    Type = SectionType.INTRO
                }
            }
        };
        
        var subject = GetSubject();
        var result = subject.GetPrompts(new TrackModel
        {
            TrackLength = new TimeSpan(0, 0, 30),
            Sections = sections
        }, new SettingsFake
        {
            PromptLengthInSeconds = 30
        });

        Assert.Single(result.Prompts);
    }
    
    [Fact]
    public void GetPrompt_SectionIsLongerThanLengthOfPromptLength_ReturnsPromptWithSubPrompts()
    {
        var sections = new Dictionary<int, SectionModel>
        {
            {
                0, new SectionModel(24)
                {
                    StartTime = new TimeOnly(0, 0, 0),
                    EndTime = new TimeOnly(0, 2, 15),
                    Type = SectionType.INTRO
                }
            }
        };
        
        var subject = GetSubject();
        var result = subject.GetPrompts(new TrackModel
        {
            TrackLength = new TimeSpan(0, 0, 30),
            Sections = sections
        }, 
            new SettingsFake
        {
            PromptLengthInSeconds = 30
        });

        Assert.Equal(5, result.Prompts.Count);
    }
    
    private PromptGenerator GetSubject()
    {
        return new PromptGenerator();
    }
}