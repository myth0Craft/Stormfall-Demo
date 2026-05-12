using System.Collections.Generic;

public static class PlayerData
{


    public static int saveIndex = 0;
    private static PlayerControls globalControls = new PlayerControls();

    public static int maxHealth = 5;
    public static int currentHealth = 5;
    public static bool gamePaused = false;
    private static bool _allowGameInput = false;
    private static bool _allowWindowInput = true;
    public static string currentScene = "1_Ancient_Springs";
    //public static Vector2 currentPosition = new Vector2(-3.0f, -1.0f);
    public static float posX = -5.0f;
    public static float posY = -1.7f;

    public static bool inWater = false;

    //ABILITIES

    public static bool dashUnlocked = false;
    public static bool sprintUnlocked = false;
    public static bool swordUnlocked = false;
    public static bool doubleJumpUnlocked = false;
    public static bool wallJumpUnlocked = false;
    public static bool shieldUnlocked = false;
    public static bool shieldBounceUnlocked = false;



    //GAME SETTINGS
    public static bool VsyncEnabled = true;
    public static bool fullScreenEnabled = true;


    //NPC DATA
    public static HashSet<string> talkedToNPCs = new HashSet<string>();

    public static bool HasTalkedTo(string npcId)
    {
        return talkedToNPCs.Contains(npcId);
    }

    public static void MarkTalkedTo(string npcId)
    {
        if (!talkedToNPCs.Contains(npcId))
        {
            talkedToNPCs.Add(npcId);
        }

    }

    public static PlayerControls getControls()
    {
        return globalControls;
    }

    public static void AllowGameInput(bool allowGameInput)
    {
        _allowGameInput = allowGameInput;
        if (allowGameInput)
        {

            globalControls.Player.Enable();
        }
        else
        {
            globalControls.Player.Disable();
        }
    }

    public static void AllowWindowInput(bool allowWindowInput)
    {
        _allowWindowInput = allowWindowInput;

        if (allowWindowInput)
        {
            globalControls.Window.Enable();
        }
        else
        {
            globalControls.Window.Disable();
        }
    }

    public static void Save(ref PlayerSaveData data)
    {
        data.posX = posX;
        data.posY = posY;
        data.maxHealth = maxHealth;
        data.currentScene = currentScene;


        //ABILITIES
        data.dashUnlocked = dashUnlocked;
        data.sprintUnlocked = sprintUnlocked;
        data.swordUnlocked = swordUnlocked;
        data.doubleJumpUnlocked = doubleJumpUnlocked;
        data.wallJumpUnlocked = wallJumpUnlocked;
        data.shieldUnlocked = shieldUnlocked;
        data.shieldBounceUnlocked = shieldBounceUnlocked;

        //NPC DATA
        data.talkedToNPCs = new List<string>(talkedToNPCs);



    }

    public static void Load(PlayerSaveData data)
    {
        posX = data.posX;
        posY = data.posY;
        maxHealth = data.maxHealth;
        currentScene = data.currentScene;
        currentHealth = data.maxHealth;

        //ABILITIES
        dashUnlocked = data.dashUnlocked;
        sprintUnlocked = data.sprintUnlocked;
        swordUnlocked = data.swordUnlocked;
        doubleJumpUnlocked = data.doubleJumpUnlocked;
        wallJumpUnlocked = data.wallJumpUnlocked;
        shieldUnlocked = data.shieldUnlocked;
        shieldBounceUnlocked = data.shieldBounceUnlocked;






        //NPC DATA
        talkedToNPCs = new HashSet<string>(data.talkedToNPCs ?? new List<string>());
    }

    public static void SetDefaults()
    {
        posX = -5.0f;
        posY = -1.7f;
        currentScene = "1_Ancient_Springs";
        maxHealth = 5;
        currentHealth = 5;

        //ABILITIES

        dashUnlocked = false;
        sprintUnlocked = false;
        swordUnlocked = false;
        doubleJumpUnlocked = false;
        wallJumpUnlocked = false;
        shieldUnlocked = false;
        shieldBounceUnlocked = false;

        //NPC DATA
        talkedToNPCs = new HashSet<string>();
    }

    public static void SaveSettingsData(ref PlayerSettingsData data)
    {
        data.vsyncEnabled = VsyncEnabled;
        data.fullscreenEnabled = fullScreenEnabled;
    }

    public static void LoadSettingsData(PlayerSettingsData data)
    {
        VsyncEnabled = data.vsyncEnabled;
        fullScreenEnabled = data.fullscreenEnabled;
    }

    public static void SetSettingsDefaults()
    {
        VsyncEnabled = true;
        fullScreenEnabled = true;
    }

}

[System.Serializable]
public struct PlayerSaveData
{
    public float posX;
    public float posY;
    public int maxHealth;
    public string currentScene;

    //ABILITIES
    public bool dashUnlocked;
    public bool sprintUnlocked;
    public bool swordUnlocked;
    public bool doubleJumpUnlocked;
    public bool wallJumpUnlocked;
    public bool shieldUnlocked;
    public bool shieldBounceUnlocked;

    //NPC DATA
    public List<string> talkedToNPCs;
}





[System.Serializable]
public struct PlayerSettingsData
{
    public bool vsyncEnabled;
    public bool fullscreenEnabled;
}
