using System.Diagnostics;
using System.Numerics;
using System.Security.Cryptography;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore.Mathematics;
using SRCore.Models.EntitySpawn;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Source : RegionPosition
{
    public static Source FromPacket(Packet packet)
    {
        var result = new Source();
        var regionId = packet.ReadUShort();
        result.RegionId = new RegionId(regionId);

        if (regionId < short.MaxValue)
        {
            result.XOffset = packet.ReadShort() / 10f;
            result.YOffset = packet.ReadFloat();
            result.ZOffset = packet.ReadShort() / 10f;
        }
        else
        {
            result.XOffset = packet.ReadInt() / 10f;
            result.YOffset = packet.ReadFloat();
            result.ZOffset = packet.ReadInt() / 10f;
        }

        return result;
    }

    public Destination ToDestination()
    {
        return new Destination
        {
            RegionId = RegionId,
            XOffset = XOffset,
            YOffset = YOffset,
            ZOffset = ZOffset
        };
    }
}

public class Destination : RegionPosition
{
    public static Destination FromPacket(Packet packet)
    {
        var result = new Destination();
        var regionId = packet.ReadUShort();

        result.RegionId = new RegionId(regionId);
        if (regionId < short.MaxValue)
        {
            result.XOffset = packet.ReadShort();
            result.YOffset = packet.ReadShort();
            result.ZOffset = packet.ReadShort();
        }
        else
        {
            result.XOffset = packet.ReadInt();
            result.YOffset = packet.ReadInt();
            result.ZOffset = packet.ReadInt();
        }

        return result;
    }

    public Source ToSource()
    {
        return new Source
        {
            RegionId = RegionId,
            XOffset = XOffset,
            YOffset = YOffset,
            ZOffset = ZOffset
        };
    }
}

public class Movement(EntityBionic bionic) : ReactiveObject
{
    [Reactive] public Destination? Destination { get; internal set; }
    [Reactive] public byte Type { get; internal set; }
    [Reactive] public MovementSourceType SourceType { get; internal set; }
    [Reactive] public float Angle { get; internal set; }
    [Reactive] public Source Source { get; internal set; } = new Source{XOffset = bionic.Position.XOffset, YOffset = bionic.Position.YOffset, ZOffset = bionic.Position.ZOffset};

    public bool AtDestination => Destination != null && Vector3.Distance(Source.World, Destination.World) < 1;
    
    internal static Movement FromPacketNoSource(EntityBionic bionic, Packet packet)
    {
        var result = new Movement(bionic);

        var hasDestination = packet.ReadBool();
        result.Type = packet.ReadByte();

        if (hasDestination)
            result.Destination = Models.Destination.FromPacket(packet);
        else
        {
            result.SourceType = (MovementSourceType)packet.ReadByte();
            result.Angle = packet.ReadUShort();
        }

        return result;
    }

    internal static Movement FromPacketWithSource(EntityBionic bionic, Packet packet)
    {
        var result = new Movement(bionic);

        var hasDestination = packet.ReadBool();
        if (hasDestination)
            result.Destination = Models.Destination.FromPacket(packet);
        else
        {
            result.SourceType = (MovementSourceType)packet.ReadByte();
            result.Angle = packet.ReadUShort() / 10000f;
        }

        var hasSource = packet.ReadBool();
        if (hasSource)
        {
            result.Source = Models.Source.FromPacket(packet);
        }

        return result;
    }

    #region Tracking


    public void Stop()
    {
        Destination = new Destination()
        {
            RegionId = bionic.Position.RegionId,
            XOffset = bionic.Position.XOffset,
            YOffset = bionic.Position.YOffset,
            ZOffset = bionic.Position.ZOffset
        };
    }

    private DateTime _lastUpdate = DateTime.Now;
    internal void Update()
    {
        if (AtDestination || Destination == null)
            return;

        var deltaTime = (float)(DateTime.Now - _lastUpdate).TotalMilliseconds;
        var destination = RegionId.Transform(Destination.Local, Destination.RegionId, Source.RegionId);
        var direction = Vector3.Normalize(destination - Source.Local);

        var newPosition = Source.Local + direction * bionic.State.Speed * deltaTime; 
        var newSource = new Source
        {
            RegionId = Source.RegionId,
            XOffset = newPosition.X,
            YOffset = newPosition.Y,
            ZOffset = newPosition.Z
        };
        newSource.Normalize();

        bionic.Position = Source.ToOrientedPosition(Angle);

        Source = newSource;

        _lastUpdate = DateTime.Now;
    }

    #endregion

    public override string ToString()
    {
        return
            $"Type: {Type}, Source: {Source.World}, Angle: {Angle}, Destination: {Destination?.World}, Speed: {bionic.State.Speed}";
    }
}