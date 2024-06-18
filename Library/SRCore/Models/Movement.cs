using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.EntitySpawn;
using SRGame;
using SRNetwork.SilkroadSecurityApi;
using System.Threading;

namespace SRCore.Models;

public class Movement(EntityPosition? source = null, RegionPosition? destination = null) : ReactiveObject
{
    [Reactive] public RegionPosition? Destination { get; internal set; }
    [Reactive] public byte Type { get; internal set; }
    [Reactive] public MovementSourceType SourceType { get; internal set; }
    [Reactive] public float Angle { get; internal set; }
    [Reactive] public RegionPosition? Source { get; internal set; } = source?.ToRegionPosition();
    [Reactive] public float Speed { get; internal set; } = 0f;


    private CancellationTokenSource _cancellationTokenSource;
    private Task? _movementTask = null;

    private EntityPosition _position;
    internal static Movement FromPacketNoSource(EntityBionic bionic, Packet packet)
    {
        var result = new Movement();

        var hasDestination = packet.ReadBool();
        result.Type = packet.ReadByte();

        if (hasDestination)
        {
            result.Speed = bionic.State.MotionState switch
            {
                MotionState.Run => bionic.State.RunSpeed,
                MotionState.Walk => bionic.State.WalkSpeed,
                _ => bionic.State.WalkSpeed
            };
            result.Destination = RegionPosition.FromPacket(packet);
        }
        else
        {
            result.SourceType = (MovementSourceType)packet.ReadByte();
            result.Angle = packet.ReadUShort();
        }

        return result;
    }

    internal static Movement FromPacketWithSource(Packet packet)
    {
        var result = new Movement();

        var hasDestination = packet.ReadBool();
        if (hasDestination)
        {
            result.Destination = RegionPosition.FromPacket(packet);
        }
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

    internal TimeSpan RemainingTime;
    public void Start(EntityPosition outputPosition, float speed = 50)
    {
        if (Source == null || Destination == null)
            return;

        Speed = speed;
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        Task.Run(() => TrackMovement(outputPosition, token), token);
    }

    public void Stop()
    {
        _cancellationTokenSource?.Cancel();
    }

    private async Task TrackMovement(EntityPosition outputPosition, CancellationToken token)
    {
        var startTime = DateTime.UtcNow;
        var startPosition = Source;
        var endPosition = Destination;
        var totalDistance = startPosition.DistanceTo(endPosition);
        var totalTime = totalDistance / Speed;

        while (!token.IsCancellationRequested)
        {
            var elapsedTime = (DateTime.UtcNow - startTime).TotalSeconds;
            if (elapsedTime >= totalTime)
            {
                Source = Destination;
                break;
            }

            var progress = elapsedTime / totalTime;
            var newX = startPosition.XOffset + (endPosition.XOffset - startPosition.XOffset) * progress;
            var newZ = startPosition.ZOffset + (endPosition.ZOffset - startPosition.ZOffset) * progress;
            Source = new RegionPosition
            {
                RegionId = startPosition.RegionId,
                XOffset = (float) newX,
                YOffset = startPosition.YOffset,
                ZOffset = (float) newZ
            };

            outputPosition.X = (float) newX;
            outputPosition.Z = (float) newZ;

            await Task.Delay(100, token);
        }
    }


    internal double MovingX, MovingY;

    #endregion

    public override string ToString()
    {
        return $"Type: {Type}, SourceType: {SourceType}, Angle: {Angle}, Position: {_position}, Destination: {Destination}";
    }
}