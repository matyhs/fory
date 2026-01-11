using Fory.Core.HashAlgorithm;

namespace Fory.Core.Tests.HashAlgorithm;

public class MurmurHash3AlgorithmTests
{
    [Theory]
    [InlineData("", new ulong[] { 0, 0 })]
    [InlineData("1", new ulong[] { 8213365047359667313, 10676604921780958775 })]
    [InlineData("12", new ulong[] { 5355690773644049813, 9855895140584599837 })]
    [InlineData("123", new ulong[] { 10978418110857903978, 4791445053355511657 })]
    [InlineData("1234", new ulong[] { 619023178690193332, 3755592904005385637 })]
    [InlineData("12345", new ulong[] { 2375712675693977547, 17382870096830835188 })]
    [InlineData("123456", new ulong[] { 16435832985690558678, 5882968373513761278 })]
    [InlineData("1234567", new ulong[] { 3232113351312417698, 4025181827808483669 })]
    [InlineData("12345678", new ulong[] { 4272337174398058908, 10464973996478965079 })]
    [InlineData("123456789", new ulong[] { 4360720697772133540, 11094893415607738629 })]
    [InlineData("123456789a", new ulong[] { 12594836289594257748, 2662019112679848245 })]
    [InlineData("123456789ab", new ulong[] { 6978636991469537545, 12243090730442643750 })]
    [InlineData("123456789abc", new ulong[] { 211890993682310078, 16480638721813329343 })]
    [InlineData("123456789abcd", new ulong[] { 12459781455342427559, 3193214493011213179 })]
    [InlineData("123456789abcde", new[] { 12538342858731408721, 9820739847336455216 })]
    [InlineData("123456789abcdef", new ulong[] { 9165946068217512774, 2451472574052603025 })]
    [InlineData("123456789abcdef1", new[] { 9259082041050667785, 12459473952842597282 })]
    public void Should_ComputeHash(string value, ulong[] hash)
    {
        // Arrange
        ReadOnlySpan<byte> bytes = System.Text.Encoding.UTF8.GetBytes(value);

        // Act
        var result = MurmurHash3Algorithm.ComputeHash(ref bytes, 0);

        // Assert
        Assert.Equal(hash[0], result.Item1);
        Assert.Equal(hash[1], result.Item2);
    }
}
