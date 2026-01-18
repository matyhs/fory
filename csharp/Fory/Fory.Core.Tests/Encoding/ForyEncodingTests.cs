using System.IO.Pipelines;
using Fory.Core.Encoding;

namespace Fory.Core.Tests.Encoding;

public class ForyEncodingTests
{
    [Theory]
    [InlineData(0)] // 1-byte min value
    [InlineData(127)] // 1-byte max value
    [InlineData(128)] // 2-byte min value
    [InlineData(16383)] // 2-byte max value
    [InlineData(16384)] // 3-byte min value
    [InlineData(2097151)] // 3-byte max value
    [InlineData(2097152)] // 4-byte min value
    [InlineData(268435455)] // 4-byte max value
    [InlineData(268435456)] // 5-byte min value
    [InlineData(4294967295)] // 5-byte max value/unsigned int max value
    public async Task Should_Encode_Decode_VarInt32(uint input)
    {
        // Arrange
        var pipe = new Pipe();

        // Act
        var byteResult = ForyEncoding.AsVarUInt32(input);
        await pipe.Writer.WriteAsync(byteResult);
        await pipe.Writer.CompleteAsync();
        var result = await ForyEncoding.FromVarUInt32Async(pipe.Reader);

        // Assert
        Assert.Equal(input, result);
    }
}
