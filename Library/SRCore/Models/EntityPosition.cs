using System.Numerics;
using ReactiveUI.Fody.Helpers;
using SRCore.Mathematics;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class EntityPosition
{
    [Reactive] public RegionId RegionId { get; internal set; } = RegionId.Empty;
    [Reactive] public float X { get; internal set; }
    [Reactive] public float Y { get; internal set; }
    [Reactive] public float Z { get; internal set; }
    [Reactive] public ushort Angle { get; internal set; }

    public float XCoordinate => X == 0 ? 0 : RegionId.IsDungeon ? X / 10 : (RegionId.X - 135) * 192 + X / 10;
    public float YCoordinate => Z == 0 ? 0 : RegionId.IsDungeon ? Z / 10 : (RegionId.Z - 92) * 192 + Z / 10;

    public Vector3 Local => new Vector3(X, Y, Z);
    public Vector3 World => Vector3.Transform(Local, RegionId.LocalToWorld);

    public RegionPosition ToRegionPosition()
    {
        return new RegionPosition
        {
            RegionId = RegionId,
            XOffset = (uint)X,
            YOffset = (uint)Y,
            ZOffset = (uint)Z
        };
    }

    public new static EntityPosition FromPacket(Packet packet)
    {
        var result = new EntityPosition
        {
            RegionId = new RegionId(packet.ReadUShort()),
            X = packet.ReadFloat(),
            Y = packet.ReadFloat(),
            Z = packet.ReadFloat(),
            Angle = packet.ReadUShort()
        };

        return result;
    }

    public float DistanceTo(EntityPosition other)
    {
        return Vector3.Distance(World, other.World);
    }

    public float DistanceTo(Vector3 other)
    {
        return Vector3.Distance(World, other);
    }

    public float DistanceTo(RegionPosition other)
    {
        return Vector3.Distance(World, other.World);
    }

    public override string ToString()
    {
#if DEBUG
        return $"RegionId: {RegionId}, X: {X}, Y: {Y}, Z: {Z}, Angle: {Angle}";
#endif

        return $"X: {X}, Y: {Y}, Z: {Z}";
    }
}