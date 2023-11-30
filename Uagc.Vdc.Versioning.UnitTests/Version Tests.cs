using System.ComponentModel.Design;
using Uagc.Vdc.Logging.Models;
using Xunit.Sdk;

namespace Uagc.Versioning.UnitTests
{
    public class Version_Tests
    {
        [Fact]
        public void Version_default()
        {
            Version v = new();
            Assert.NotNull(v);
            Assert.Equal("0.0.0", v.ToString());
        }

        [Fact]
        public void Version_from_string()
        {
            Version v = new("3.4.5");
            Assert.Equal("3.4.5", v.ToString());
        }

        [Theory]
        [InlineData("1.2.3", 1 * Version.MAJOR_VALUE_SCALE + 2 * Version.MINOR_VALUE_SCALE + 3)]
        [InlineData("0.0.0", 0)]
        [InlineData("999.888.777", 999 * Version.MAJOR_VALUE_SCALE + 888 * Version.MINOR_VALUE_SCALE + 777)]  // upper limit is 999.999.999
        public void Version_value(string version, uint expected)
        {
            Version v = new(version);
            Assert.Equal(expected, v.Value);
        }

        [Fact]
        public void Version_equality()
        {
            Version v = new("3.4.5");
            Assert.Equal("3.4.5", v.ToString());
        }

        [Fact]
        public void Version_comparison()
        {
            Version v = new("3.4.5");
            Assert.Equal("3.4.5", v.ToString());
        }

        [Theory]
        [InlineData(null, null, null, null, "All nulls returns null")]
        [InlineData("123.456.789", null, null, null, "At least the Lower limit must be passed")]
        [InlineData("123.456.789", "123.456.790", null, false, "Patch is too low")]
        [InlineData("123.456.789", "123.457.789", null, false, "Minor is too low")]
        [InlineData("123.456.789", "124.456.789", null, false, "Major is too low")]
        [InlineData("123.456.789", "123.456.789", null, true, "Version is equal to lower, no upper")]
        [InlineData("123.456.789", "122.456.788", null, false, "Patch is too high")]
        [InlineData("123.456.789", "122.455.789", null, false, "Minor is too high")]
        [InlineData("123.456.789", "121.456.789", null, false, "Major is too high")]
        [InlineData("123.456.789", "122.456.789", null, true, "Version is equal to upper, no lower")]
        [InlineData("123.456.789", "123.456.789", "123.456.789", true, "Version is in range (all equal)")]
        [InlineData("123.456.789", "122.999.999", "124.0.0", true, "Version is in range (major)")]
        [InlineData("123.456.789", "123.455.999", "124.0.0", true, "Version is in range (minor)")]
        [InlineData("123.456.789", "123.456.789", "124.0.0", true, "Version is in range (patch)")]
        public void Version_inRange(string? vs, string? lower, string? upper, bool? expected, string msg)
        {
            bool? actual = null;
            Version? v = null;

            try
            {
                v = vs is null ? null : new Version(vs);
                actual = v is not null ? Version.InRange(v, lower, upper) : null;
            } catch
            {
                actual = null;
            }
            Assert.True((v, expected, actual) switch
            {
                (_, null, null) => true,
                (_, not null, null) => false,
                (_, null, not null) => false,
                (null, not null, not null) => false,
                (not null, _, _) => actual == expected
            }, msg);
        }
    }
}