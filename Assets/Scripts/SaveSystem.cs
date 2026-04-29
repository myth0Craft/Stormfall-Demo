using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveSystem
{
    private static SaveData saveData = new SaveData();

    private static SettingsData settingsData = new SettingsData();

    [System.Serializable]
    public class SaveData
    {
        public PlayerSaveData playerData;
        public Dictionary<string, RoomSaveData> roomData = new();
    }


    [System.Serializable]
    public class SettingsData
    {
        public PlayerSettingsData playerSettingsData;
    }

    public static RoomSaveData getRoom(string sceneId)
    {
        if (!saveData.roomData.TryGetValue(sceneId, out var room))
        {
            room = new RoomSaveData();
            saveData.roomData.Add(sceneId, room);
        }
        return room;
    }


    public static string SaveFileName(int saveIndex)
    {

        saveIndex = Mathf.Clamp(saveIndex, 0, 3) + 1;

        string saveFile = Application.persistentDataPath + "/save" + saveIndex + ".json";
        return saveFile;
    }

    public static string SettingsFileName()
    {
        string settingsFile = Application.persistentDataPath + "/game_settings.json";
        return settingsFile;
    }

    public static void Save(int saveIndex)
    {
        PlayerData.Save(ref saveData.playerData);
        //File.WriteAllText(SaveFileName(saveIndex), JsonUtility.ToJson(saveData, true));
        File.WriteAllText(SaveFileName(saveIndex), JsonConvert.SerializeObject(saveData, Formatting.Indented));
        SaveSettingsData();
        
    }

    public static void SaveSettingsData()
    {
        PlayerData.SaveSettingsData(ref settingsData.playerSettingsData);

        File.WriteAllText(SettingsFileName(), JsonConvert.SerializeObject(settingsData, Formatting.Indented));

    }


    public static void LoadSettingsData()
    {
        if (File.Exists(SettingsFileName()))
        {
            string settingsSaveContent = File.ReadAllText(SettingsFileName());

            settingsData = JsonConvert.DeserializeObject<SettingsData>(settingsSaveContent);
            PlayerData.LoadSettingsData(settingsData.playerSettingsData);
        }
        else
        {
            PlayerData.SetSettingsDefaults();
            PlayerData.SaveSettingsData(ref settingsData.playerSettingsData);
            File.WriteAllText(SettingsFileName(), JsonConvert.SerializeObject(settingsData, Formatting.Indented));
        }
    }

    public static void Load(int saveIndex)
    {
        if (File.Exists(SaveFileName(saveIndex)))
        {
            string saveContent = File.ReadAllText(SaveFileName(saveIndex));
            //saveData = JsonUtility.FromJson<SaveData>(saveContent);

            saveData = JsonConvert.DeserializeObject<SaveData>(saveContent);
            PlayerData.Load(saveData.playerData);
        } else
        {
            PlayerData.SetDefaults();
            PlayerData.Save(ref saveData.playerData);
            saveData.roomData = new();
            //File.WriteAllText(SaveFileName(saveIndex), JsonUtility.ToJson(saveData, true));
            File.WriteAllText(SaveFileName(saveIndex), JsonConvert.SerializeObject(saveData, Formatting.Indented));
        }
    }


    public static void ResetSaveFile(int saveIndex)
    {
        File.Delete(SaveFileName(saveIndex));

        /*PlayerData.SetDefaults();
        PlayerData.Save(ref saveData.playerData);
        saveData.roomData = new();
        //File.WriteAllText(SaveFileName(saveIndex), JsonUtility.ToJson(saveData, true));
        File.WriteAllText(SaveFileName(saveIndex), JsonConvert.SerializeObject(saveData, Formatting.Indented));*/
    }

    public static bool CheckIfFileExists(int saveIndex)
    {
        return File.Exists(SaveFileName(saveIndex));
    }

}
