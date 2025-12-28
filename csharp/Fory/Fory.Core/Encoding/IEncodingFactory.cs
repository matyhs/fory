namespace Fory.Core.Encoding
{
    public interface IEncodingFactory
    {
        (System.Text.Encoding Encoding, byte Flag) GetEncoding(string value);
    }
}
