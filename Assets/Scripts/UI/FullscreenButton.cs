using TMPro;
using UnityEngine;

public class FullscreenButton : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            text.text = "Fullscreen: Enabled";
        } else
        {
            text.text = "Fullscreen: Disabled";
        }
    }

    public void ButtonChangeFullscreenMode()
    {
        WindowManager.instance.ToggleScreenState();
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            text.text = "Fullscreen: Enabled";
        }
        else
        {
            text.text = "Fullscreen: Disabled";
        }
    }



}
