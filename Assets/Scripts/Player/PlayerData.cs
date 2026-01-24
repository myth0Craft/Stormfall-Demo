using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public static class PlayerData
{
    public static int saveIndex = 0;
    private static PlayerControls globalControls = new PlayerControls();
    public static int maxHealth = 5;
    public static int currentHealth = 5;
    public static bool gamePaused = false;
    private static bool _allowGameInput = false;
    public static string currentScene = "1_Ancient_Springs";
    //public static Vector2 currentPosition = new Vector2(-3.0f, -1.0f);
    public static float posX = -5.0f;
    public static float posY = -1.7f;


    public static PlayerControls getControls()
    {
        return globalControls;
    }

    public static void AllowGameInput(bool allowGameInput)
    {
        _allowGameInput = allowGameInput;
        if (allowGameInput)
        {
            globalControls.Enable();
        } else
        {
            globalControls.Disable();
        }
    }

    public static void Save(ref PlayerSaveData data)
    {
        data.posX = posX;
        data.posY = posY;
        data.maxHealth = maxHealth;
        data.currentScene = currentScene;
    }

    public static void Load(PlayerSaveData data)
    {
        posX = data.posX;
        posY = data.posY;
        maxHealth = data.maxHealth;
        currentScene = data.currentScene;
        currentHealth = data.maxHealth;
    }

    public static void SetDefaults()
    {
        posX = -5.0f;
        posY = -1.7f;
        currentScene = "1_Ancient_Springs";
        maxHealth = 5;
        currentHealth = 5;
    }
}

[System.Serializable]
public struct PlayerSaveData
{
    public float posX;
    public float posY;
    public int maxHealth;
    public string currentScene;
}
