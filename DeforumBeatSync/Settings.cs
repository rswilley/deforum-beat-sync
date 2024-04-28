using System.Text.Json.Serialization;

namespace DeforumBeatSync;

public interface ISettings
{
    int Fps { get; init; }
    int PromptLengthInSeconds { get; init; }
    int BarFrameCount { get; init; }
    int HalfNoteFrameCount { get; }
    int QuarterNoteFrameCount { get; }
    StrengthSettings Strength { get; init; }
    double TranslationX { get; init; }
    double TranslationY { get; init; }
    double TranslationZ { get; init; }
    double RotationX { get; init; }
    double RotationY { get; init; }
    double RotationZ { get; init; }
}

public class Settings : ISettings
{
    [JsonPropertyName("fps")]
    public int Fps { get; init; }
    [JsonPropertyName("promptLengthInSeconds")]
    public int PromptLengthInSeconds { get; init; }
    
    [JsonPropertyName("barFrameCount")]
    public int BarFrameCount { get; init; }
    public int HalfNoteFrameCount => BarFrameCount / 2;
    public int QuarterNoteFrameCount => BarFrameCount / 4;

    [JsonPropertyName("strength")] 
    public StrengthSettings Strength { get; init; } = new();
    
    // 3D Translation X
    // Translation X moves the camera sideways.
    // A positive value moves the camera to the right. A negative value moves the camera to the left.
    [JsonPropertyName("translationX")]
    public double TranslationX { get; init; }
    
    // 3D Translation Y
    // Translation Y moves the camera up and down.
    // Using a positive value moves the camera up. A negative value moves the camera down.
    [JsonPropertyName("translationY")]
    public double TranslationY { get; init; }
    
    // 3D Translation Z
    // Translation Z in 3D is similar to zoom in 2D motions.
    [JsonPropertyName("translationZ")]
    public double TranslationZ { get; init; }
    
    // 3D rotation X
    // Rotation X rotates the camera about the X-axis.
    [JsonPropertyName("rotationX")]
    public double RotationX { get; init; }
    
    // 3D rotation Y
    // Rotation Y rotates the camera about the Y-axis.
    [JsonPropertyName("rotationY")]
    public double RotationY { get; init; }
    
    // 3D rotation Z
    // Rotation Z rotates the camera about the Z-axis.
    [JsonPropertyName("rotationZ")]
    public double RotationZ { get; init; }
}

public class StrengthSettings
{
    [JsonPropertyName("high")]
    public double High { get; init; }
    [JsonPropertyName("low")]
    public double Low { get; init; }
    [JsonPropertyName("constant")]
    public double Constant { get; init; }
}