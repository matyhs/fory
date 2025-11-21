namespace Fory.Core.Encoding
{
    public class AllToLowerSpecialEncoding : System.Text.Encoding
    {
        public override int GetByteCount(char[] chars, int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public override int GetBytes(char[] chars, int charIndex, int charCount, byte[] bytes, int byteIndex)
        {
            throw new System.NotImplementedException();
        }

        public override int GetCharCount(byte[] bytes, int index, int count)
        {
            throw new System.NotImplementedException();
        }

        public override int GetChars(byte[] bytes, int byteIndex, int byteCount, char[] chars, int charIndex)
        {
            throw new System.NotImplementedException();
        }

        public override int GetMaxByteCount(int charCount)
        {
            throw new System.NotImplementedException();
        }

        public override int GetMaxCharCount(int byteCount)
        {
            throw new System.NotImplementedException();
        }
    }
}
