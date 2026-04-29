using UnityEngine;

public class WindowManager : MonoBehaviour
{
    public static WindowManager instance;
    public PlayerControls controls;
    private bool fullscreenPressed = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }

        controls = PlayerData.getControls();
        controls.Window.Fullscreen.performed += OnFullscreen;
        PlayerData.AllowWindowInput(true);
    }

    private void OnDestroy()
    {
        controls.Window.Fullscreen.performed -= OnFullscreen;
    }

    private void OnFullscreen(UnityEngine.InputSystem.InputAction.CallbackContext ctx)
    {
        ToggleScreenState();
        Debug.Log("Fullscreen Toggled");
    }


    public void SetFullscreen()
    {
        Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
        PlayerData.fullScreenEnabled = true;
        SaveSystem.SaveSettingsData();
        Canvas.ForceUpdateCanvases();
    }

    public void SetWindowed()
    {
        Screen.fullScreenMode = FullScreenMode.Windowed;
        PlayerData.fullScreenEnabled = false;
        SaveSystem.SaveSettingsData();
        Canvas.ForceUpdateCanvases();
    }

    public void ToggleScreenState()
    {
        if (Screen.fullScreenMode == FullScreenMode.Windowed)
        {
            Resolution res = Screen.currentResolution;
            Screen.SetResolution(res.width, res.height, FullScreenMode.FullScreenWindow);

            PlayerData.fullScreenEnabled = true;

        }
        else
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            PlayerData.fullScreenEnabled = false;
        }
        Canvas.ForceUpdateCanvases();
        SaveSystem.SaveSettingsData();
    }


}
