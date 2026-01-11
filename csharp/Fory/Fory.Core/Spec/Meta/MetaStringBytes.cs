namespace Fory.Core.Spec.Meta;

internal class MetaStringBytes
{
    public MetaStringBytes(byte[] bytes, byte encoding, long hashCode)
    {
        Bytes = bytes;
        Encoding = encoding;
        HashCode = hashCode;
    }

    public byte[] Bytes { get; }
    public byte Encoding { get; }
    public long HashCode { get; }

    public override int GetHashCode()
    {
        return Bytes.GetHashCode() + Encoding.GetHashCode() + HashCode.GetHashCode();
    }
}
