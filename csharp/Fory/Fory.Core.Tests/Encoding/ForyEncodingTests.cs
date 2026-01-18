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
    [InlineData(uint.MaxValue)] // 5-byte max value/unsigned int max value
    public async Task Should_Encode_Decode_VarUInt32(uint input)
    {
        // Arrange
        var pipe = new Pipe();

        // Act
        var byteResult = ForyEncoding.AsVarUInt32(input).ToArray();
        await pipe.Writer.WriteAsync(byteResult);
        await pipe.Writer.CompleteAsync();
        var result = await ForyEncoding.FromVarUInt32Async(pipe.Reader);

        // Assert
        Assert.Equal(input, result);
    }

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
    [InlineData(uint.MaxValue)] // 5-byte unsigned int max value
    [InlineData(34359738367)] // 5-byte max value
    [InlineData(34359738368)] // 6-byte min value
    [InlineData(4398046511103)] // 6-byte max value
    [InlineData(4398046511104)] // 7-byte min value
    [InlineData(562949953421311)] // 7-byte max value
    [InlineData(562949953421312)] // 8-byte min value
    [InlineData(72057594037927935)] // 8-byte max value
    [InlineData(72057594037927936)] // 9-byte min value
    [InlineData(9223372036854775807)] // 9-byte max value
    public async Task Should_Encode_Decode_VarUInt64(ulong input)
    {
        // Arrange
        var pipe = new Pipe();

        // Act
        var byteResult = ForyEncoding.AsVarUInt64(input).ToArray();
        await pipe.Writer.WriteAsync(byteResult);
        await pipe.Writer.CompleteAsync();
        var result = await ForyEncoding.FromVarUInt64Async(pipe.Reader);

        // Assert
        Assert.Equal(input, result);
    }

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
    [InlineData(uint.MaxValue)] // 5-byte unsigned int max value
    [InlineData(34359738367)] // 5-byte max value
    [InlineData(34359738368)] // 6-byte min value
    [InlineData(68719476735)] // 36-bit max value
    public async Task Should_Encode_Decode_VarUInt36(ulong input)
    {
        // Arrange
        var pipe = new Pipe();

        // Act
        var byteResult = ForyEncoding.AsVarUInt64(input).ToArray();
        await pipe.Writer.WriteAsync(byteResult);
        await pipe.Writer.CompleteAsync();
        var result = await ForyEncoding.FromVarUInt64Async(pipe.Reader);

        // Assert
        Assert.Equal(input, result);
    }
}
