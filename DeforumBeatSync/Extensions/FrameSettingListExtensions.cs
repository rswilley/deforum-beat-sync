using System.Text;

namespace DeforumBeatSync.Extensions;

public static class FrameSettingListExtensions
{
    public static string ToSchedule(this IEnumerable<FrameSetting> frameSettings)
    {
        var sb = new StringBuilder();
        foreach (var item in frameSettings)
        {
            sb.Append($"{item.FrameNumber}: ({item.FrameValue}), ");
        }
        return sb.ToString().TrimEnd(',').TrimEnd(' ');
    }
}