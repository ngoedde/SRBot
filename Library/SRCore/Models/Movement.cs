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

    public bool AtDestination => (Destination != null && SourceType == MovementSourceType.Default) &&
                                 bionic.Position.DistanceTo(Destination) < 0.01;

    internal static Movement FromPacketNoSource(EntityBionic bionic, Packet packet)
    {
        var result = new Movement(bionic);

        var hasDestination = packet.ReadBool();
        result.Type = (MovementType)packet.ReadByte();

        if (hasDestination)
            result.Destination = ReadDestination(packet);
        else
        {
            result.SourceType = (MovementSourceType)packet.ReadByte();
            result.Angle = packet.ReadUShort();
        }

        return result;
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
        if (hasSource)
        {
            var source = ReadSource(packet).ToOrientedPosition(bionic.Position.Angle);

            bionic.Position = source;
        }
    }

    #region Tracking

    internal void TackPosition(long deltaTime)
    {
        if (AtDestination)
            return;

        var speed = Type switch
        {
            MovementType.Walk => bionic.State.WalkSpeed * Constants.Scale,
            MovementType.Run => bionic.State.RunSpeed * Constants.Scale,
            _ => 0
        };

        var newPosition = bionic.Position.Local;
        var source = bionic.Position.Local;
        
        if (SourceType == MovementSourceType.Default)
        {
            if (Destination == null)
                return;
            
            if (bionic.Position.RegionId != Destination!.RegionId)
            {

            }

            var destination = RegionId.Transform(Destination.Local, Destination.RegionId, bionic.Position.RegionId);
            var distanceToDestination = Vector3.Distance(bionic.Position.Local, destination); //Distance between regions is too high! > 200 with actual 2
            
            var direction = Vector3.Normalize(destination - source);
            var time = (float)deltaTime / TimeSpan.TicksPerSecond;

            newPosition = source + direction * speed * time;

            var distanceTraveled = Vector3.Distance(source, newPosition);

            //Destination reached
            if (distanceTraveled == 0 || distanceTraveled >= distanceToDestination)
            {
                newPosition = destination;

                Destination = null;
            }
        } 
        else if (SourceType == MovementSourceType.SkyClick)
        {
            var angle = Angle * MathF.PI / short.MaxValue;
            var direction = new Vector3(MathF.Cos(angle), 0, MathF.Sin(angle));

            newPosition = source + direction * speed * deltaTime / TimeSpan.TicksPerSecond;
        }

        bionic.Position.XOffset = newPosition.X;
        bionic.Position.YOffset = newPosition.Y;
        bionic.Position.ZOffset = newPosition.Z;
        bionic.Position.Normalize();
    }

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