namespace DeforumBeatSync.Tests;

public class BarTypeResolverTests
{
    [Fact]
    public void GetType_BarIsBreakdown_ReturnBreakdown()
    {
        var beats = new List<Beat>
        {
            new()
            {
                FrameSetting = new FrameSetting
                {
                    FrameNumber = 1,
                    FrameValue = 0.14   
                },
            },
            new()
            {
                FrameSetting = new FrameSetting
                {
                    FrameNumber = 2,
                    FrameValue = 0.13   
                }
            },
            new()
            {
                FrameSetting = new FrameSetting
                {
                    FrameNumber = 3,
                    FrameValue = 0.13   
                }
            },
            new()
            {
                FrameSetting = new FrameSetting
                {
                    FrameNumber = 4,
                    FrameValue = 0.12   
                }
            }
        };
        
        var subject = GetSubject();
        var type = subject.GetType(beats);
        
        Assert.Equal(BarType.BREAKDOWN, type);
    }
    
    [Fact]
    public void GetType_BarIsBuildup_ReturnBuildup()
    {
        var beats = new List<Beat>
        {
            new()
            {
                FrameSetting = new FrameSetting
                {
                    FrameNumber = 1,
                    FrameValue = 0.14   
                },
            },
            new()
            {
                FrameSetting = new FrameSetting
                {
                    FrameNumber = 2,
                    FrameValue = 0.13   
                }
            },
            new()
            {
                FrameSetting = new FrameSetting
                {
                    FrameNumber = 3,
                    FrameValue = 0.13   
                }
            },
            new()
            {
                FrameSetting = new FrameSetting
                {
                    FrameNumber = 4,
                    FrameValue = 0.12   
                }
            }
        };
        
        var subject = GetSubject();
        var type = subject.GetType(beats);
        
        Assert.Equal(BarType.BUILDUP, type);
    }
    
    private static BarTypeResolver GetSubject()
    {
        return new BarTypeResolver();
    }
}