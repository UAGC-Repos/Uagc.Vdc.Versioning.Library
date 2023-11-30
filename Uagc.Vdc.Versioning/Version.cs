using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Uagc.Versioning;

/// <summary>
/// Provides a version number to all IT assets in production and durable entities (such as a Managed Templates template).
/// Version should be visible in the UI (or embedded in a URL).  Can be used to tie Code Commits to specific 
/// versions to make rollbacks easier.
/// </summary>
[DebuggerDisplay($"{{{nameof(GetDebuggerDisplay)}(),nq}}")]
public class Version : IEquatable<Version?>, IComparable<Version>
{
    // TODO: tsm - Move Version comment to README
    /*  
        Supports 3-level Versions ("1.2.3"), representing three types of change (ref: http://semver.org)
          1. Major - incremented when a breaking change alters the app's API.
          2. Minor - incremented when a non-breaking change adds functionality to the API.
          3. Patch - incremented when a non-breaking bug fix is implemented.
          NOTE: Patch is also incremented to record content and cosmetic changes.

        When Major is incremented, Minor and Patch are zeroed.
        When Minor is incremented, Patch is zeroed.

        ctors:
          Version() creates a "0.0.0" instance.
          Version(string version_string) parses a string in the form of "1.2.3" to create a matching Version instance.

        Exceptions:
            InvalidOperationException is thrown if Version(string version_string) is unable to parse version_string.

        Version.ChangeLog()
          Create this self-documenting code in startup that calls ChangeLog() for each feature addition or major code commit. 
          Each call increments the indicated Version part:

             // version.ChangeLog(Version.Part.MAJOR || Version.Part.Minor || Version.PATCH, string? comment) 
             version = new();
             version = Version.ChangeLog(version, Version.Part.MINOR, "Added Db Cleanup");                      // 0.1.0
             version = Version.ChangeLog(version, Version.Part.PATCH, "Added Version to Views");                // 0.1.1
             version = Version.ChangeLog(version, Version.Part.MINOR, "Integrated logging with App Insights");  // 0.2.0
             version = Version.ChangeLog(version, Version.Part.MAJOR, "Released to QA");                        // 1.0.0
             version = Version.ChangeLog(version, Version.Part.PATCH, "Fixed search bug");                      // 1.0.1

             Version 1.0.1 is released to production.

        bool Version.InRange(version, Version lowest, Version? highest = null)
          Tests 'version' to see if it fits between the inclusive lowest Version, and the inclusive highest Version. If
          highest is null, then InRange() only performs the test on 'lowest'.

            Version.InRange(version, "3.0.0")               returns true if 'version' is 3.0.0 or above.
            Version.InRange(version, "3.27.83", "7.5.55")   returns true if 'version' is within the limits.

        NOTE: Development versions should be in the range of "0.*.*". ChangeLog() MAJOR once submitted to QA.
    */

    // supporting up to 999.999.999:
    public const uint MAJOR_VALUE_SCALE = 1000000;
    public const uint MINOR_VALUE_SCALE = 1000;
    // PATCH_VALUE_SCALE = 1;

    private uint _major;
    private uint _minor;
    private uint _patch;

    public enum Part : byte
    {
        MAJOR = 0,
        MINOR,
        PATCH
    };

    public Version() { }

    public Version([DisallowNull] string vs)
    {
        if (String.IsNullOrWhiteSpace(vs)) throw new ArgumentOutOfRangeException(nameof(vs), $"No Version created, cannot parse empty input string.");
        string[] parts = vs.Split('.', StringSplitOptions.TrimEntries);
        if (parts.Length != 3) throw new ArgumentOutOfRangeException(nameof(vs), $"No Version created, unable to parse a Version from '{vs}'");
        if (!UInt32.TryParse(parts[0], out _major)) throw new InvalidOperationException($"No Version created, the Major value must resolve to a positive integer.");
        if (!UInt32.TryParse(parts[1], out _minor)) throw new InvalidOperationException($"No Version created, the Minor value must resolve to a positive integer.");
        if (!UInt32.TryParse(parts[2], out _patch)) throw new InvalidOperationException($"No Version created, the Patch value must resolve to a positive integer.");
    }

    public uint Major => _major;
    public uint Minor => _minor;
    public uint Patch => _patch;

    public uint Value => _major * MAJOR_VALUE_SCALE + _minor * MINOR_VALUE_SCALE + _patch;

    #region IEquatable, IComparable
    public override bool Equals(object? obj) => Equals(obj as Version);

    public bool Equals(Version? other) => other switch
    {
        not null => this.Value == other.Value,
        _ => false
    };

    public override int GetHashCode() => HashCode.Combine(_major, _minor, _patch);

    public int CompareTo(Version? other) => other switch
    {
        not null when other.Value == this.Value => 0,
        not null when other.Value > this.Value => 1,
        not null when other.Value < this.Value => -1,
        _ => 1
    };

    #region Operators
    public static bool operator ==(Version? left, Version? right) => (left, right) switch
    {
        (null, null) => true,
        (not null, not null) => left.Value == right.Value,
        (_, _) => false
    };

    public static bool operator !=(Version? left, Version? right) => (left, right) switch
    {
        (null, null) => false,
        (not null, not null) => left.Value != right.Value,
        (_, _) => true
    };

    public static bool operator >(Version? left, Version? right) => (left, right) switch
    {
        (null, null) => false,
        (not null, null) => true,
        (not null, not null) => left.CompareTo(right) is 1,
        (_, _) => false
    };

    public static bool operator <(Version? left, Version? right) => (left, right) switch
    {
        (null, null) => false,
        (null, not null) => true,
        (not null, not null) => left.CompareTo(right) is 1,
        (_, _) => false
    };

    public static bool operator >=(Version? left, Version? right) => (left, right) switch
    {
        (null, null) => true,
        (not null, null) => true,
        (not null, not null) => left.CompareTo(right) is 0 or 1,
        (_, _) => false
    };

    public static bool operator <=(Version? left, Version? right) => (left, right) switch
    {
        (null, null) => true,
        (null, not null) => true,
        (not null, not null) => left.CompareTo(right) is 0 or 1,
        (_, _) => false
    };
    #endregion
    #endregion

    #region Version business rules (look at this as 'room to grow')
    private uint setMajor(uint value)
    {
        if (value != _major)
        {
            setMinor(0);        // zeroes patch also
        }
        return value;
    }

    private uint setMinor(uint value)
    {
        if (value != _major)
        {
            setPatch(0);
        }
        return value;
    }

    private uint setPatch(uint value)
    {
        return value;
    }
    #endregion

    #region ChangeLog
    // TODO: tsm - Implement comment storage in Version.ChangeLog?

    static public Version ChangeLog(Version ogVer, Version.Part element, string comment)
    {
        Version newVer = new(ogVer.ToString());
        _ = element switch
        {
            Part.MAJOR => newVer.setMajor(newVer.Major + 1),
            Part.MINOR => newVer.setMinor(newVer.Minor + 1),
            Part.PATCH => newVer.setPatch(newVer.Patch + 1),
            _ => (uint)0
        };
        return newVer;
    }

    static public bool? InRange([DisallowNull] Version valueUnderTest, string? lower, string? upper)
    {
        uint? vLimit0, vLimit1;

        vLimit0 = calculateValue(lower);
        vLimit1 = calculateValue(upper);

        return (vLimit0, vLimit1) switch
        {
            (not null, null) => valueUnderTest.Value >= vLimit0,
            (not null, not null) => valueUnderTest.Value >= vLimit0 && valueUnderTest.Value <= vLimit1,
            (_, _) => null  // don't know for (null, null) and (null, not null)
        };
    }

    static private uint? calculateValue(string? version)
    {
        if (version is null) return null;
        try
        {
            Version v = new(version);
            return Convert.ToUInt32(v.Major * MAJOR_VALUE_SCALE + v.Minor * MINOR_VALUE_SCALE + v.Patch);
        }
        catch   // Version ctor() exceptions
        {
            return null;
        }
    }
    #endregion

    public override string ToString() => $"{_major}.{_minor}.{_patch}";

    private string GetDebuggerDisplay() => this.ToString();
}


