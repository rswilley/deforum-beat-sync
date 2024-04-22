namespace DeforumBeatSync.Tests;

public class BarTypeResolverTests
{
    [Fact]
    public void GetType_BarIsBreakdown_ReturnBreakdown()
    {
        var beats = new List<FrameValue>
        {
            new()
            {
                Frame = 1,
                Value = 0.14
            },
            new()
            {
                Frame = 2,
                Value = 0.13
            },
            new()
            {
                Frame = 3,
                Value = 0.13
            },
            new()
            {
                Frame = 4,
                Value = 0.12
            }
        };
        
        var subject = GetSubject();
        var type = subject.GetType(beats);
        
        Assert.Equal(BarType.BREAKDOWN, type);
    }
    
    [Fact]
    public void GetType_BarIsBuildup_ReturnBuildup()
    {
        var beats = new List<FrameValue>
        {
            new()
            {
                Frame = 1,
                Value = 0.14
            },
            new()
            {
                Frame = 2,
                Value = 0.13
            },
            new()
            {
                Frame = 3,
                Value = 0.13
            },
            new()
            {
                Frame = 4,
                Value = 0.12
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