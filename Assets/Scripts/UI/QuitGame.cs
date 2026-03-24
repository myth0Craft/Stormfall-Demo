using UnityEngine;

public class QuitGame : MonoBehaviour
{
    public void QuitGameTrigger()
    {
#if !UNITY_EDITOR
        Application.Quit();
#endif
    }
}
