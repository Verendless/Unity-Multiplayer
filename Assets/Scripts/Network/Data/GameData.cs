using System;

public enum Map
{
    Defualt
}

public enum GameMode
{
    Default
}

public enum GameQueue
{
    Solo,
    Team
}

[Serializable]
public class UserData
{
    public string userName;
    public string userAuthId;
    public int teamIndex = -1;
    public GameInfo userGamePreferences;
}

[Serializable]
public class GameInfo
{
    public Map map;
    public GameMode mode;
    public GameQueue gameQueue;

    public string ToMultiplayQueue()
    {
        return gameQueue switch
        {
            GameQueue.Solo => "solo-queue",
            GameQueue.Team => "team-queue",
            _ => "solo-queue"
        };
    }
}
