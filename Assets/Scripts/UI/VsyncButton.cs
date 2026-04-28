using TMPro;
using UnityEngine;

public class VsyncButton : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (PlayerData.VsyncEnabled)
        {
            text.text = "Vsync: Enabled";
            QualitySettings.vSyncCount = 1;
        } else
        {
            text.text = "Vsync: Disabled";
            QualitySettings.vSyncCount = 0;
        }
    }

    public void ButtonToggleVsync()
    {
        PlayerData.VsyncEnabled = !PlayerData.VsyncEnabled;
        if (PlayerData.VsyncEnabled)
        {
            text.text = "Vsync: Enabled";
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            text.text = "Vsync: Disabled";
            QualitySettings.vSyncCount = 0;
        }
        
    }



}
