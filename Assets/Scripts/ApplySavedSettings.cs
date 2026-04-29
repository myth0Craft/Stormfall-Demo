using UnityEngine;

public class ApplySavedSettings : MonoBehaviour
{
    private void Awake()
    {
        SaveSystem.LoadSettingsData();

        if (PlayerData.VsyncEnabled)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }

        if (PlayerData.fullScreenEnabled)
        {
            Screen.fullScreenMode = FullScreenMode.FullScreenWindow;

        }
        else
        {
            Screen.fullScreenMode = FullScreenMode.Windowed;
        }

    }
}
