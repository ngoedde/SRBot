namespace SRCore.Models.Quests;

public enum QuestStatus : byte
{
    //1 = Initialized
    //2 = Complete, but not supplied
    //3 = Completed
    //4 = Completed X times
    //5 = Unavailable
    //6 = Canceled
    //7 = Started by User
    //8 = Completed by User, but not supplied
    Achieving = 1,
    AchievedButNotPayed = 2,
    Achieved = 3,
    Nodata = 4,
    AchievedOnlyForServer = 5,
    LockedByAbort = 6,
    AchievingKillMonster = 7,
    AchievedKillMonster = 8,
}