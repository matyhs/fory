using Fory.Core.Encoding;

namespace Fory.Core.Tests.Encoding;

public class ForyEncodingTests
{
    [Theory]
    [InlineData(0, new byte[] { 0 })] // 1-byte min value
    [InlineData(127, new byte[] { 127 })] // 1-byte max value
    [InlineData(128, new byte[] { 128, 1 })] // 2-byte min value
    [InlineData(16383, new byte[] { 255, 127 })] // 2-byte max value
    [InlineData(16384, new byte[] { 128, 128, 1 })] // 3-byte min value
    [InlineData(2097151, new byte[] { 255, 255, 127 })] // 3-byte max value
    [InlineData(2097152, new byte[] { 128, 128, 128, 1 })] // 4-byte min value
    [InlineData(268435455, new byte[] { 255, 255, 255, 127 })] // 4-byte max value
    [InlineData(268435456, new byte[] { 128, 128, 128, 128, 1 })] // 5-byte min value
    [InlineData(4294967295, new byte[] { 255, 255, 255, 255, 15 })] // 5-byte max value/unsigned int max value
    public void Should_Encode_VarInt32(uint input, byte[] output)
    {
        // Act
        var result = ForyEncoding.AsVarInt32(input).ToArray();

        // Assert
        Assert.Equal(output, result);
    }
}
