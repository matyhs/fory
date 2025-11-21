namespace Fory.Core
{
    public class ForySerializerOptions
    {
        internal static readonly ForySerializerOptions Default = new ForySerializerOptions();

        public bool Compatible { get; set; }

        public bool Xlang { get; set; }

        public bool CompressString { get; set; }

        public bool CheckVersion { get; set; }

        public int MaxNestedDepth { get; set; } = 5;
    }
}
