using System.Numerics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore.Mathematics;
using SRCore.Models.EntitySpawn;
using SRGame;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Movement(EntityBionic bionic) : ReactiveObject
{
    [Reactive] public RegionPosition? Destination { get; internal set; }
    [Reactive] public MovementType Type { get; internal set; }
    [Reactive] public MovementSourceType SourceType { get; internal set; } = MovementSourceType.Default;
    [Reactive] public float Angle { get; internal set; }

    public float Speed
    {
        get
        {
            return Type switch
            {
                MovementType.Walk => bionic.State.WalkSpeed * Constants.Scale,
                MovementType.Run => bionic.State.RunSpeed * Constants.Scale,
                _ => 0
            };
        }
    }

    public bool AtDestination => Destination != null 
                                 && SourceType == MovementSourceType.Default
                                 && Vector2.Distance(bionic.Position.World.ToVector2(), Destination.World.ToVector2()).IsApproximatelyZero();

    internal void UpdateFromPacketNoSource(Packet packet)
    {
        var hasDestination = packet.ReadBool();
        Type = (MovementType)packet.ReadByte();

        if (hasDestination)
            Destination = ReadDestination(packet);
        else
        {
            SourceType = (MovementSourceType)packet.ReadByte();
            Angle = packet.ReadUShort();
        }
    }

    internal void UpdateFromPacket(Packet packet)
    {
        var hasDestination = packet.ReadBool();
        if (hasDestination)
        {
            SourceType = MovementSourceType.Default;
            Destination = ReadDestination(packet);
        }
        else
        {
            SourceType = (MovementSourceType)packet.ReadByte();
            Angle = packet.ReadUShort() / 10_000f;

            Destination = null;
        }

        var hasSource = packet.ReadBool();
        if (!hasSource)
            return;

        bionic.Position = ReadSource(packet).ToOrientedPosition(bionic.Position.Angle);
    }

    internal void UpdateAngleFromPacket(Packet packet)
    {
        Angle = packet.ReadUShort();
    }

    #region Tracking

    internal void Stop()
    {
        Destination = null;
        SourceType = MovementSourceType.Default;
    }

    internal void TackPosition2D(long deltaTime)
    {
        if (AtDestination)
            return;

        var newPosition = bionic.Position.World;
        var source = newPosition with { Y = 0 };
        var time = (float)deltaTime / TimeSpan.TicksPerSecond;
        
        if (SourceType == MovementSourceType.Default)
        {
            if (Destination == null)
                return;

            var destination = Destination.World with { Y = 0 };
            var distanceToDestination = Vector3.Distance(source, destination);
            var direction = Vector3.Normalize(destination - source);

            newPosition = source + direction * Speed * time;

            var distanceTraveled = Vector3.Distance(source, newPosition);

            //Destination reached
            if (distanceTraveled.IsApproximatelyZero(0.001f) || distanceTraveled >= distanceToDestination)
            {
                newPosition = destination;
                Destination = null;
            }
        }
        else if (SourceType == MovementSourceType.SkyClick)
        {
            var angle = Angle * MathF.PI / short.MaxValue;
            var direction = new Vector3(MathF.Cos(angle), 0, MathF.Sin(angle));

            newPosition = source + direction * Speed * time;
        }

        var localPosition = Vector3.Transform(newPosition, bionic.Position.RegionId.WorldToLocal);

        bionic.Position.XOffset = localPosition.X;
        bionic.Position.ZOffset = localPosition.Z;
    }

    #region WIP!
    //Attempts to handle position tracking by region positon transformation instead of using world coordinates (better accuracy due to floating point precision)
    // internal void TackPosition(long deltaTime)
    // {
    //     if (AtDestination)
    //         return;
    //     
    //     var newPosition = bionic.Position.Local;
    //     var source = newPosition with { Y = 0 };
    //     var time = (float)deltaTime / TimeSpan.TicksPerSecond;
    //     
    //     //ToDo: Better speed determination (e.g. Hwan is missing for now)
    //     var speed = Type switch
    //     {
    //         MovementType.Walk => bionic.State.WalkSpeed * Constants.Scale,
    //         MovementType.Run => bionic.State.RunSpeed * Constants.Scale,
    //         _ => 0
    //     };
    //
    //     if (SourceType == MovementSourceType.Default)
    //     {
    //         if (Destination == null)
    //             return;
    //
    //         Vector3 destination;
    //
    //         // If the destination is in a different region, transform it to the source region
    //        // destination = RegionId.TransformPoint(Destination.RegionId, bionic.Position.RegionId, Destination.Local);
    //         destination = RegionId.TransformPoint(bionic.Position.RegionId, Destination.RegionId, Destination.Local);
    //
    //         var distanceToDestination = Vector3.Distance(source, destination);
    //         var direction = Vector3.Normalize(destination - source);
    //
    //         newPosition = source + direction * speed * time;
    //
    //         var distanceTraveled = Vector3.Distance(source, newPosition);
    //
    //         //Destination reached
    //         if (distanceTraveled.IsApproximatelyZero(0.001f) || distanceTraveled >= distanceToDestination)
    //         {
    //             newPosition = destination;
    //             Destination = null;
    //         }
    //     } 
    //     else if (SourceType == MovementSourceType.SkyClick)
    //     {
    //         var angle = Angle * MathF.PI / short.MaxValue;
    //         var direction = new Vector3(MathF.Cos(angle), 0, MathF.Sin(angle));
    //
    //         newPosition = source + direction * speed * time;
    //     }
    //
    //     bionic.Position.XOffset = newPosition.X;
    //     bionic.Position.ZOffset = newPosition.Z;
    //     bionic.Position.Normalize();
    // }
    #endregion
    #endregion

    private static RegionPosition ReadSource(Packet packet)
    {
        var result = new RegionPosition();
        var regionId = new RegionId(packet.ReadUShort());
        result.RegionId = new RegionId(regionId);

        if (!regionId.IsDungeon)
        {
            result.XOffset = packet.ReadShort() / 10f * Constants.Scale;
            result.YOffset = packet.ReadFloat();
            result.ZOffset = packet.ReadShort() / 10f * Constants.Scale;
        }
        else
        {
            result.XOffset = packet.ReadInt() / 10f * Constants.Scale;
            result.YOffset = packet.ReadFloat();
            result.ZOffset = packet.ReadInt() / 10f * Constants.Scale;
        }

        return result;
    }

    private static RegionPosition ReadDestination(Packet packet)
    {
        var result = new RegionPosition();
        var regionId = new RegionId(packet.ReadUShort());

        result.RegionId = regionId;
        if (!regionId.IsDungeon)
        {
            result.XOffset = packet.ReadShort() * Constants.Scale;
            result.YOffset = packet.ReadShort();
            result.ZOffset = packet.ReadShort() * Constants.Scale;
        }
        else
        {
            result.XOffset = packet.ReadInt() * Constants.Scale;
            result.YOffset = packet.ReadInt();
            result.ZOffset = packet.ReadInt() * Constants.Scale;
        }

        return result;
    }

    public override string ToString()
    {
        return
            $"Type: {Type}, Source: {bionic.Position.World}, Angle: {Angle}, Destination: {Destination?.World}";
    }
}