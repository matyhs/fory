using System.Collections.Concurrent;

namespace Fory.Core.Tests;

public partial class ForyTests
{
    [Fact]
    public void Should_Serialize_Deserialize_UInt32_Multithread()
    {
        // Arrange
        uint[] values = [0, 1, ushort.MinValue, ushort.MaxValue, uint.MinValue, uint.MaxValue];
        var result = new ConcurrentBag<uint>();
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        Parallel.ForEach(values, new ParallelOptions { MaxDegreeOfParallelism = values.Length }, async void (value) =>
        {
            var bufferResult = await fory.SerializeAsync(value);
            var actual = await fory.DeserializeAsync<uint>(bufferResult);
            result.Add(actual);
        });

        // Assert
        Assert.NotEmpty(result);
        Assert.Equal(values.Length, result.Count);
    }
}
