namespace Fory.Core.Tests;

public class ForyTests
{
    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Serialize_Deserialize_Boolean(bool value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions() { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var result = await fory.DeserializeAsync<bool>(bufferResult);

        // Assert
    }
}
