using System.Collections.ObjectModel;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using SRCore.Models.Skills;
using SRGame;
using SRGame.Client;
using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models;

public class State : ReactiveObject
{
    [Reactive] public LifeState LifeState { get; internal set; }
    [Reactive] public MotionState MotionState { get; internal set; }
    [Reactive] public BodyState BodyState { get; internal set; }
    [Reactive] public float WalkSpeed { get; internal set; }
    [Reactive] public float RunSpeed { get; internal set; }
    [Reactive] public float HwanSpeed { get; internal set; }
    [Reactive] public ObservableCollection<Buff> Buffs { get; internal set; } = new();

    internal void UpdateSpeedFromPacket(Packet packet)
    {
        WalkSpeed = packet.ReadFloat();
        RunSpeed = packet.ReadFloat();
    }

    internal void UpdateFromPacket(Packet packet, EntityManager entityManager)
    {
        LifeState = (LifeState)packet.ReadByte();
        packet.ReadByte(); //Unknown
        MotionState = (MotionState)packet.ReadByte();
        BodyState = (BodyState)packet.ReadByte();
        WalkSpeed = packet.ReadFloat();
        RunSpeed = packet.ReadFloat();
        HwanSpeed = packet.ReadFloat();

        var buffCount = packet.ReadByte();
        for (var i = 0; i < buffCount; i++)
        {
            var skillId = packet.ReadInt();
            var refSkill = entityManager.GetSkill(skillId);
            if (refSkill == null)
                throw new Exception($"Skill {skillId} not found");

            var buff = new Buff(refSkill)
            {
                Duration = packet.ReadUInt()
            };

            if (refSkill.Params.Contains(1701213281))
            {
                buff.IsCreator = packet.ReadBool();
            }

            Buffs.Add(buff);
        }
    }
}