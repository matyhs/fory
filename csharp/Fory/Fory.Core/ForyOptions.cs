namespace Fory.Core;

public class ForyOptions
{
    internal static readonly ForyOptions Default = new();

    public bool Compatible { get; set; }

    public bool Xlang { get; set; }

    public bool CompressString { get; set; }

    public bool CheckVersion { get; set; }

    public int MaxNestedDepth { get; set; } = 5;
}
