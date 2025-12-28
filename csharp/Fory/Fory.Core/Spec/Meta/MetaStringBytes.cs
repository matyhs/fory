namespace Fory.Core.Spec.Meta
{
    internal class MetaStringBytes
    {
        public byte[] Bytes { get; }
        public byte Encoding { get; }
        public long HashCode { get; }

        public MetaStringBytes(byte[] bytes, byte encoding, long hashCode)
        {
            Bytes = bytes;
            Encoding = encoding;
            HashCode = hashCode;
        }

        public override int GetHashCode()
        {
            return Bytes.GetHashCode() + Encoding.GetHashCode() + HashCode.GetHashCode();
        }
    }
}
