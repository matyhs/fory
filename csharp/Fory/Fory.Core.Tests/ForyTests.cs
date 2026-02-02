namespace Fory.Core.Tests;

public partial class ForyTests
{
    [Fact]
    public async Task Should_Serialize_Deserialize_WithStream()
    {
        // Arrange
        const string value = "this is a test to write to the provided stream";
        var fory = new Fory();

        // Act
        using var stream = new MemoryStream();
        await fory.SerializeAsync(value, stream);
        stream.Position = 0L;
        var result = await fory.DeserializeAsync<string>(stream);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(value, result);
    }
}
