using TMPro;
using UnityEngine;

public class FullscreenButton : MonoBehaviour
{
    private TextMeshProUGUI text;
    private PlayerControls controls;

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
        controls = PlayerData.getControls();
        controls.Window.Fullscreen.performed += UpdateTextEvent;
    }

    public void ButtonChangeFullscreenMode()
    {
        WindowManager.instance.ToggleScreenState();
        UpdateText();
    }

    private void UpdateTextEvent(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        UpdateText();
    }

    private void UpdateText()
    {
        if (Screen.fullScreenMode == FullScreenMode.FullScreenWindow)
        {
            text.text = "Fullscreen: Disabled";
            
        }
        else
        {
            text.text = "Fullscreen: Enabled";
            
        }
    }



}
