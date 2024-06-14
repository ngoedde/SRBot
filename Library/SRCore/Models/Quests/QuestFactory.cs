using SRNetwork.SilkroadSecurityApi;

namespace SRCore.Models.Quests;

internal class QuestFactory
{
    public static Quest ParseFromPacket(Packet packet)
    {
        var result = new Quest
        {
            RefQuestId = packet.ReadUInt(),
            AchievementCount = packet.ReadByte(),
            RequiresAutoShareParty = packet.ReadBool(),
            Type = (QuestType)packet.ReadByte(),
        };

        if ((result.Type & QuestType.Time) != 0)
            result.RemainingTime = packet.ReadUInt();

        if ((result.Type & QuestType.Status) != 0)
            result.Status = (QuestStatus)packet.ReadByte();

        if ((result.Type & QuestType.Objective) != 0)
        {
            var objectiveCount = packet.ReadByte();
            for (var i = 0; i < objectiveCount; i++)
            {
                var objective = new QuestObjective()
                {
                    Id = packet.ReadByte(),
                    Status = (QuestObjectiveStatus)packet.ReadByte(),
                    Name = packet.ReadString(),
                };

                var taskCount = packet.ReadByte();
                objective.TaskValues = new uint[taskCount];
                for (var j = 0; j < taskCount; j++)
                    objective.TaskValues[j] = packet.ReadUInt();

                result.Objectives.Add(objective);
            }
        }

        if ((result.Type & QuestType.RefObjects) != 0)
        {
            var npcCount = packet.ReadByte();
            result.NpcIds = new uint[npcCount];
            for (var i = 0; i < npcCount; i++)
                result.NpcIds[i] = packet.ReadUInt();
        }

        return result;
    }
}