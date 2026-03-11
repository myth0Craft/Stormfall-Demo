using System.Collections.Generic;


[System.Serializable]
public static class RoomData
{





    public static void Save(ref RoomSaveData data)
    {

    }


    public static void Load(RoomSaveData data)
    {

    }

   
}


[System.Serializable]
public class RoomSaveData
{
    public Dictionary<string, bool> breakables = new();
    public Dictionary<string, bool> pickups = new();
    public Dictionary<string, int> switches = new();
}
