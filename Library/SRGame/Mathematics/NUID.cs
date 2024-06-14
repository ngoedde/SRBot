using System.Runtime.InteropServices;

namespace SRGame.Mathematics;

[StructLayout(LayoutKind.Explicit)]
public struct NUID : System.IEquatable<NUID>
{
    [FieldOffset(0)] private readonly uint _id;

    [FieldOffset(0)] private readonly RegionId _regionId;

    [FieldOffset(2)] private readonly ushort _localID;

    public RegionId RegionId => _regionId;
    public ushort LocaUID => _localID;

    public NUID(uint worldUID)
    {
        _regionId = default;
        _localID = default;
        _id = worldUID;
    }

    public NUID(RegionId regionID, ushort localUID)
    {
        _id = default;
        _regionId = regionID;
        _localID = localUID;
    }

    public override bool Equals(object? obj) => obj is NUID id && this.Equals(id);

    public bool Equals(NUID other) => _id == other._id;

    public override int GetHashCode() => _id.GetHashCode();

    public static bool operator ==(NUID left, NUID right) => left.Equals(right);

    public static bool operator !=(NUID left, NUID right) => !(left == right);

    public static implicit operator uint(NUID value) => value._id;

    public override string ToString() => $"{_localID} - {_regionId}";
}