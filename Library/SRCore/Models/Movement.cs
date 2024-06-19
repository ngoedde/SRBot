using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.EntitySpawn;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class Movement(OrientedRegionPosition source) : ReactiveObject
{
    [Reactive] public RegionPosition? Destination { get; internal set; }
    [Reactive] public byte Type { get; internal set; }
    [Reactive] public MovementSourceType SourceType { get; internal set; }
    [Reactive] public float Angle { get; internal set; }
    [Reactive] public RegionPosition Source { get; internal set; } = source;
    [Reactive] public float Speed { get; internal set; }
    
    private readonly CancellationTokenSource _cancellationTokenSource = new();
    
    internal static Movement FromPacketNoSource(EntityBionic bionic, Packet packet)
    {
        var result = new Movement(bionic.Position);

        var hasDestination = packet.ReadBool();
        result.Type = packet.ReadByte();

        if (hasDestination)
            result.Destination = RegionPosition.FromPacket(packet);
        else
        {
            result.SourceType = (MovementSourceType)packet.ReadByte();
            result.Angle = packet.ReadUShort();
        }

        return result;
    }

    internal static Movement FromPacketWithSource(EntityBionic bionic, Packet packet)
    {
        var result = new Movement(bionic.Position);

        var hasDestination = packet.ReadBool();
        if (hasDestination)
            result.Destination = RegionPosition.FromPacket(packet);
        else
        {
            result.SourceType = (MovementSourceType)packet.ReadByte();
            result.Angle = packet.ReadUShort();
        }

        var hasSource = packet.ReadBool();
        if (hasSource)
        {
            result.Source = RegionPosition.FromPacket(packet);
        }

        return result;
    }

    #region Tracking

    public void Start(OrientedRegionPosition outputPosition, float speed = 50)
    {
        Speed = speed;
        
        var token = _cancellationTokenSource.Token;

        Task.Run(() => TrackMovement(outputPosition, token), token);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

    private async Task TrackMovement(OrientedRegionPosition outputPosition, CancellationToken token)
    {
        var startTime = DateTime.UtcNow;
        var startPosition = Source;
        var endPosition = Destination;

        if (endPosition != null)
        {
            var totalDistance = startPosition.DistanceTo(endPosition);
            var totalTime = totalDistance / Speed;

            while (!token.IsCancellationRequested)
            {
                var elapsedTime = (DateTime.UtcNow - startTime).TotalSeconds;
                if (elapsedTime >= totalTime)
                {
                    Source = Destination!;
   
                    return;
                }

                var progress = elapsedTime / totalTime;
                var newX = startPosition.XOffset + (endPosition.XOffset - startPosition.XOffset) * progress;
                var newZ = startPosition.ZOffset + (endPosition.ZOffset - startPosition.ZOffset) * progress;
                Source = new RegionPosition
                {
                    RegionId = startPosition.RegionId,
                    XOffset = (float)newX,
                    YOffset = startPosition.YOffset,
                    ZOffset = (float)newZ
                };

                outputPosition.XOffset = (float)newX;
                outputPosition.ZOffset = (float)newZ;

                await Task.Delay(100, token);
            }
        }
        else //Sky click
        {
            var angleInRadians = Angle * (Math.PI / 180); // Convert angle to radians
            while (!token.IsCancellationRequested)
            {
                var elapsedTime = (DateTime.UtcNow - startTime).TotalSeconds;
                var distanceTravelled = Speed * elapsedTime;
                var newX = startPosition.XOffset + Math.Cos(angleInRadians) * distanceTravelled;
                var newZ = startPosition.ZOffset + Math.Sin(angleInRadians) * distanceTravelled;
                Source = new RegionPosition
                {
                    RegionId = startPosition.RegionId,
                    XOffset = (float)newX,
                    YOffset = startPosition.YOffset,
                    ZOffset = (float)newZ
                };

                outputPosition.XOffset = (float)newX;
                outputPosition.ZOffset = (float)newZ;

                await Task.Delay(100, token);
            }
        }
    }

    #endregion

    public override string ToString()
    {
        return
            $"Type: {Type}, Source: {Source.World}, Angle: {Angle}, Destination: {Destination?.World}, Speed: {Speed}";
    }
}