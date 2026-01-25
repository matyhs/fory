/*
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at
 *
 *   http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

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

    [Theory]
    [InlineData("a sample sentence.")]
    [InlineData("email@email.com")]
    [InlineData("    ")]
    [InlineData("""

                 {
                    "glossary": {
                        "title": "example glossary",
                		"GlossDiv": {
                            "title": "S",
                			"GlossList": {
                                "GlossEntry": {
                                    "ID": "SGML",
                					"SortAs": "SGML",
                					"GlossTerm": "Standard Generalized Markup Language",
                					"Acronym": "SGML",
                					"Abbrev": "ISO 8879:1986",
                					"GlossDef": {
                                        "para": "A meta-markup language, used to create markup languages such as DocBook.",
                						"GlossSeeAlso": ["GML", "XML"]
                                    },
                					"GlossSee": "markup"
                                }
                            }
                        }
                    }
                }

                """)] // Example from https://json.org/example.html
    public async Task Should_Serialize_Deserialize_String(string value)
    {
        // Arrange
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<string>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    [InlineData(long.MinValue)]
    [InlineData(long.MaxValue)]
    public async Task Should_Serialize_Deserialize_Duration(long ticks)
    {
        // Arrange
        var value = TimeSpan.FromTicks(ticks);
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<TimeSpan>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public async Task Should_Serialize_Deserialize_Timestamp(long ticks)
    {
        // Arrange
        var value = new DateTimeOffset(ticks + DateTimeOffset.UnixEpoch.Ticks, TimeSpan.Zero);
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<DateTimeOffset>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(short.MinValue)]
    [InlineData(short.MaxValue)]
    [InlineData(int.MinValue)]
    [InlineData(int.MaxValue)]
    public async Task Should_Serialize_Deserialize_LocalDate(long ticks)
    {
        // Arrange
        var value = new DateTimeOffset(ticks + DateTimeOffset.UnixEpoch.Ticks, TimeSpan.Zero).Date;
        var fory = new Fory(new ForyOptions { Xlang = true });

        // Act
        var bufferResult = await fory.SerializeAsync(value);
        var actual = await fory.DeserializeAsync<DateTime>(bufferResult);

        // Assert
        Assert.Equal(value, actual);
    }
}
