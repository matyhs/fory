namespace Fory.Core.Tests;

public class ForyTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Serialize_Deserialize_Boolean(bool value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<bool>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(65)]
    [InlineData(sbyte.MinValue)]
    [InlineData(sbyte.MaxValue)]
    public async Task Should_Serialize_Deserialize_Int8(sbyte value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<sbyte>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(sbyte.MinValue)]
    [InlineData(sbyte.MaxValue)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    public async Task Should_Serialize_Deserialize_Int16(short value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<short>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public async Task Should_Serialize_Deserialize_Int32(int value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<int>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public async Task Should_Serialize_Deserialize_Int64(long value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<long>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-124.355)]
    [InlineData(234.67)]
    [InlineData(-65504)]
    [InlineData(65504)]
    public async Task Should_Serialize_Deserialize_Float16(Half value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<Half>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(-124.355)]
    [InlineData(234.67)]
    [InlineData(float.MinValue)]
    [InlineData(float.MaxValue)]
    public async Task Should_Serialize_Deserialize_Float32(float value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<float>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(float.MinValue)]
    [InlineData(float.MaxValue)]
    [InlineData(double.MinValue)]
    [InlineData(double.MaxValue)]
    public async Task Should_Serialize_Deserialize_Float64(double value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<double>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(65)]
    [InlineData(byte.MinValue)]
    [InlineData(byte.MaxValue)]
    public async Task Should_Serialize_Deserialize_UInt8(byte value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<byte>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(byte.MinValue)]
    [InlineData(byte.MaxValue)]
    [InlineData(ushort.MinValue)]
    [InlineData(ushort.MaxValue)]
    public async Task Should_Serialize_Deserialize_UInt16(ushort value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<ushort>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(ushort.MinValue)]
    [InlineData(ushort.MaxValue)]
    [InlineData(uint.MinValue)]
    [InlineData(uint.MaxValue)]
    public async Task Should_Serialize_Deserialize_UInt32(uint value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<uint>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(uint.MinValue)]
    [InlineData(uint.MaxValue)]
    [InlineData(ulong.MinValue)]
    [InlineData(ulong.MaxValue)]
    public async Task Should_Serialize_Deserialize_UInt64(ulong value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<ulong>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }
}
