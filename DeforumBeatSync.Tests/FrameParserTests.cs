using Moq;

namespace DeforumBeatSync.Tests;

public class FrameParserTests
{
    private readonly Mock<IFileAdapter> _fileReaderMock = new();
    
    [Fact]
    public void ReadFile_ByDefault_ReturnParsedFrames()
    {
        const string fileContents = @"    0:(0.00),    1:(0.00),    2:(0.25),    3:(0.21),    4:(0.07),    5:(0.06),    6:(0.05),    7:(0.04),    8:(0.04),    9:(0.04),   10:(0.04),   11:(0.03),   12:(0.03),   13:(0.12),   14:(0.34),   15:(0.05),   16:(0.04),   17:(0.03),   18:(0.02),   19:(0.02),   20:(0.02),   21:(0.02),   22:(0.02),   23:(0.02),
   24:(0.04),   25:(0.39),   26:(0.06),   27:(0.03),   28:(0.02),   29:(0.01),   30:(0.01),   31:(0.01),   32:(0.01),   33:(0.01),   34:(0.01),   35:(0.01),   36:(0.36),   37:(0.12),   38:(0.02),   39:(0.01),   40:(0.01),   41:(0.01),   42:(0.01),   43:(0.01),   44:(0.01),   45:(0.01),   46:(0.01),   47:(0.24)";
        
        _fileReaderMock.Setup(x => x.ReadFile(It.IsAny<string>())).ReturnsAsync(fileContents);
        
        var subject = GetSubject();
        var results = subject.ReadFrames(fileContents);
        
        Assert.Equal(48, results.Count);
        Assert.Equal(0, results.ElementAt(0).Key);
        Assert.Equal(0, results.ElementAt(0).Value.FrameNumber);
        Assert.Equal(0.00d, results.ElementAt(0).Value.FrameValue);
    }

    private static FrameParser GetSubject()
    {
        return new FrameParser();
    }
}