using UnityEngine;

public class EventSystemController : MonoBehaviour
{
    [SerializeField] private GameObject eventSystem;


    private void Start()
    {
        eventSystem.SetActive(true);
    }
    public void blockRaycast()
    {
        eventSystem.SetActive(false);
    }

    public void StartGameLoad(int saveIndex)
    {
        SceneLoader.instance.LoadGame(saveIndex);
    }

    public void StartLoadTitle()
    {
        SceneLoader.instance.LoadTitleScreen();
    }
}
