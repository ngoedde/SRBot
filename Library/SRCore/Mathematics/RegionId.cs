using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;

namespace SRCore.Mathematics;

[DebuggerDisplay("{_value} [{this.X}x{this.Z}]")]
[Serializable]
public struct RegionId : IEquatable<RegionId>
{
    public const float Width = 1920f * Constants.Scale;
    public const float Length = 1920f * Constants.Scale;

    public const int CenterX = 135;
    public const int CenterZ = 92;
    public static readonly RegionId Center = new RegionId(CenterX, CenterZ);
    public static readonly RegionId Empty = new RegionId(0);
    public static readonly RegionId Invalid = new RegionId(-1);

    #region Reasons to use C++

    private const int X_SIZE = 8;
    private const int X_OFFSET = 0;
    private const ushort X_MASK = ((1 << X_SIZE) - 1) << X_OFFSET;

    private const int Z_SIZE = 7;
    private const int Z_OFFSET = X_OFFSET + X_SIZE;
    private const ushort Z_MASK = ((1 << Z_SIZE) - 1) << Z_OFFSET;

    private const int DUNGEON_SIZE = 1;
    private const int DUNGEON_OFFSET = Z_OFFSET + Z_SIZE;
    private const ushort DUNGEON_MASK = ((1 << DUNGEON_SIZE) - 1) << DUNGEON_OFFSET;

    #endregion Reasons to use C++

    //(MSB)                                                                       (LSB)
    // | 15 | 14 | 13 | 12 | 11 | 10 | 09 | 08 | 07 | 06 | 05 | 04 | 03 | 02 | 01 | 00 |
    // | DB |               Z (0-127)          |              X (0 - 255)              |
    // DB = DungeonBit
    private ushort _value;

    #region Properties

    public byte X
    {
        get { return Convert.ToByte((_value & X_MASK) >> X_OFFSET); }
        set { _value = (ushort)((_value & ~X_MASK) | ((value << X_OFFSET) & X_MASK)); }
    }

    public byte Z
    {
        get { return Convert.ToByte((_value & Z_MASK) >> Z_OFFSET); }
        set { _value = (ushort)((_value & ~Z_MASK) | ((value << Z_OFFSET) & Z_MASK)); }
    }

    public bool IsDungeon
    {
        get { return Convert.ToBoolean((_value & DUNGEON_MASK) >> DUNGEON_OFFSET); }
        set
        {
            _value = (ushort)((_value & ~DUNGEON_MASK) | ((Convert.ToByte(value) << DUNGEON_OFFSET) & DUNGEON_MASK));
        }
    }

    public Vector3 Position => new Vector3(this.WorldX, 0.0f, this.WorldZ);

    public Matrix4x4 LocalToWorld => Matrix4x4.CreateTranslation(this.WorldX, 0, this.WorldZ);
    public Matrix4x4 WorldToLocal => Matrix4x4.CreateTranslation(-this.WorldX, 0, -this.WorldZ);
    public static Vector3 Transform(Vector3 value, RegionId sourceRegion, RegionId targetRegion)
    {
        if (sourceRegion != targetRegion)
        {
            var localX = value.X + ((targetRegion.X - sourceRegion.X) * Width);
            var localZ = value.Z + ((targetRegion.Z - sourceRegion.Z) * Length);

            value = new Vector3(localX, 0, localZ);
        }
        return value;
    }
    public float WorldX
    {
        get
        {
            if (this.IsDungeon)
                return 0.0f;

            return (this.X /*- CenterX*/) * Width;
        }
    }

    public float WorldZ
    {
        get
        {
            if (this.IsDungeon)
                return 0.0f;

            return (this.Z /*- CenterZ*/) * Length;
        }
    }

    #endregion Properties

    #region Constructors

    public RegionId(short id)
    {
        _value = unchecked((ushort)id);
    }

    public RegionId(ushort id)
    {
        _value = id;
    }

    public RegionId(byte rx, byte rz, bool isDungeon = false)
    {
        _value = 0;

        this.X = rx;
        this.Z = (byte)(rz & sbyte.MaxValue);
        this.IsDungeon = isDungeon;
    }

    #endregion Constructors

    #region Methods

    public override string ToString() => $"{_value} [{this.X}x{this.Z}]";

    public override bool Equals(object? obj)
    {
        if (obj is RegionId region)
            return this.Equals(region);

        return false;
    }

    public bool Equals(RegionId other) => _value == other._value;

    public override int GetHashCode() => _value.GetHashCode();

    public RegionId[] Get4Neighbors()
    {
        return new RegionId[4]
        {
            new RegionId(this.X, (byte)(this.Z + 1)), //TopMid
            new RegionId((byte)(this.X - 1), this.Z), //Left
            new RegionId((byte)(this.X + 1), this.Z), //Right
            new RegionId(this.X, (byte)(this.Z - 1)), //BottomMid
        };
    }

    public void Get4Neighbors(Span<RegionId> rids)
    {
        Debug.Assert(rids.Length >= 4, "Span is too short");

        rids[0] = new RegionId(this.X, (byte)(this.Z + 1)); //TopMid
        rids[1] = new RegionId((byte)(this.X - 1), this.Z); //Left
        rids[2] = new RegionId((byte)(this.X + 1), this.Z); //Right
        rids[3] = new RegionId(this.X, (byte)(this.Z - 1)); //BottomMid
    }

    public RegionId[] Get8Neighbors()
    {
        return new RegionId[8]
        {
            new RegionId((byte)(this.X - 1), (byte)(this.Z + 1)), //TopLeft
            new RegionId(this.X, (byte)(this.Z + 1)), //TopMid
            new RegionId((byte)(this.X + 1), (byte)(this.Z + 1)), //TopRight

            new RegionId((byte)(this.X - 1), this.Z), //Left
            new RegionId((byte)(this.X + 1), this.Z), //Right

            new RegionId((byte)(this.X - 1), (byte)(this.Z - 1)), //BottomLeft
            new RegionId(this.X, (byte)(this.Z - 1)), //BottomMid
            new RegionId((byte)(this.X + 1), (byte)(this.Z - 1)), //BottomRight
        };
    }

    public void Get8Neighbors(Span<RegionId> rids)
    {
        Debug.Assert(rids.Length >= 8, "Span is too short");

        rids[1] = new RegionId((byte)(this.X - 1), (byte)(this.Z + 1)); //TopLeft
        rids[2] = new RegionId(this.X, (byte)(this.Z + 1)); //TopMid
        rids[3] = new RegionId((byte)(this.X + 1), (byte)(this.Z + 1)); //TopRight

        rids[4] = new RegionId((byte)(this.X - 1), this.Z); //MidLeft
        rids[5] = new RegionId((byte)(this.X + 1), this.Z); //MidRight

        rids[6] = new RegionId((byte)(this.X - 1), (byte)(this.Z - 1)); //BottomLeft
        rids[7] = new RegionId(this.X, (byte)(this.Z - 1)); //BottomMid
        rids[8] = new RegionId((byte)(this.X + 1), (byte)(this.Z - 1)); //BottomRight
    }

    public RegionId[] Get9Neighbors()
    {
        return new RegionId[9]
        {
            new RegionId((byte)(this.X - 1), (byte)(this.Z + 1)), //TopLeft
            new RegionId(this.X, (byte)(this.Z + 1)), //TopMid
            new RegionId((byte)(this.X + 1), (byte)(this.Z + 1)), //TopRight

            new RegionId((byte)(this.X - 1), this.Z), //MidLeft
            new RegionId(this.X, this.Z), //MidMid
            new RegionId((byte)(this.X + 1), this.Z), //MidRight

            new RegionId((byte)(this.X - 1), (byte)(this.Z - 1)), //BottomLeft
            new RegionId(this.X, (byte)(this.Z - 1)), //BottomMid
            new RegionId((byte)(this.X + 1), (byte)(this.Z - 1)), //BottomRight
        };
    }

    public void Get9Neighbors(Span<RegionId> rids)
    {
        Debug.Assert(rids.Length >= 9, "Span is too short");

        rids[1] = new RegionId((byte)(this.X - 1), (byte)(this.Z + 1)); //TopLeft
        rids[2] = new RegionId(this.X, (byte)(this.Z + 1)); //TopMid
        rids[3] = new RegionId((byte)(this.X + 1), (byte)(this.Z + 1)); //TopRight

        rids[4] = new RegionId((byte)(this.X - 1), this.Z); //MidLeft
        rids[5] = new RegionId(this.X, this.Z); //MidMid
        rids[6] = new RegionId((byte)(this.X + 1), this.Z); //MidRight

        rids[7] = new RegionId((byte)(this.X - 1), (byte)(this.Z - 1)); //BottomLeft
        rids[8] = new RegionId(this.X, (byte)(this.Z - 1)); //BottomMid
        rids[9] = new RegionId((byte)(this.X + 1), (byte)(this.Z - 1)); //BottomRight
    }

    #endregion Methods

    public static RegionId Create(byte x, byte z) => new RegionId(x, z);

    public static Vector3 TransformPoint(RegionId source, RegionId target, Vector3 offset)
    {
        if (source == target)
            return offset;

        var localX = ((target.X - source.X) * Width) + offset.X;
        var localY = offset.Y;
        var localZ = ((target.Z - source.Z) * Length) + offset.Z;
        return new Vector3(localX, localY, localZ);
    }

    public static Matrix4x4 TransformMatrix(RegionId source, RegionId target, Matrix4x4 matrix)
    {
        if (source == target)
            return matrix;

        var localX = (target.X - source.X) * Width;
        var localZ = (target.Z - source.Z) * Length;
        return Matrix4x4.CreateTranslation(localX, 0, localZ) * matrix;
    }

    #region Operators

    public static implicit operator short(RegionId region) => unchecked((short)region._value);

    public static explicit operator RegionId(short value) => new RegionId(value);

    public static implicit operator ushort(RegionId region) => region._value;

    public static explicit operator RegionId(ushort value) => new RegionId(value);

    public static bool operator ==(RegionId left, RegionId right) => left.Equals(right);

    public static bool operator !=(RegionId left, RegionId right) => !(left == right);

    public static bool operator >(RegionId left, RegionId right) => left.X > right.X && left.Z > right.Z;

    public static bool operator <(RegionId left, RegionId right) => left.X < right.X && left.Z < right.Z;

    public static bool operator >=(RegionId left, RegionId right) => left.X >= right.X && left.Z >= right.Z;

    public static bool operator <=(RegionId left, RegionId right) => left.X <= right.X && left.Z <= right.Z;

    public static RegionId operator +(RegionId left, RegionId right) =>
        new RegionId((byte)(left.X + right.X), (byte)(left.Z + right.Z));

    public static RegionId operator -(RegionId left, RegionId right) =>
        new RegionId((byte)(left.X - right.X), (byte)(left.Z - right.Z));

    #endregion Operators
}