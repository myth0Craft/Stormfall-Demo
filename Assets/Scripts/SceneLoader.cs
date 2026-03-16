using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private string startScene = "1_Ancient_Springs";
    private string persistentGame = "PersistentData";
    //private FaderController fader;
    public static SceneLoader instance { get; private set; }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }

        startScene = PlayerData.currentScene;
        //fader = GetComponentInChildren<FaderController>();
        
    }


    public void LoadTitleScreen()
    {
        SaveSystem.Save(PlayerData.saveIndex);
        PlayerData.AllowGameInput(false);
        
        Time.timeScale = 0;
        StartCoroutine(LoadTitleScreenCoroutine());
    }

    public IEnumerator LoadTitleScreenCoroutine()
    {
        yield return FaderController.instance.FadeOut();
        FaderController.instance.setOpaque();
        PlayerData.gamePaused = false;
        SceneManager.LoadScene("Title");
        Time.timeScale = 1;

        /*EventSystem eventSystem = EventSystem.current;
        while (eventSystem == null)
        {
            eventSystem = EventSystem.current;
            yield return null;
        }

        eventSystem.enabled = false;*/
        yield return FaderController.instance.FadeIn();
        //eventSystem.enabled = true;
        
        //Destroy(this.gameObject);
    }

    /*public IEnumerator UnloadAllScenes()
    {

        int numActiveScenes = SceneManager.loadedSceneCount;

        for (int i = 0; i < numActiveScenes; i++)
        {

            Debug.Log("unloading scene at index " + i);

            yield return SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(i));
            
        }

    }*/


    //public void LoadGame()
    //{
    //    StartCoroutine(fader.FadeOut());
    //    SceneManager.UnloadSceneAsync("Title");
    //    SceneManager.LoadSceneAsync(persistentGame);

    //    if (!SceneManager.GetSceneByName(startScene).isLoaded)
    //        SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);
    //    //fader.FadeOut();
    //}

    public void LoadGame(int saveIndex)
    {
        PlayerData.saveIndex = saveIndex;
        
        Time.timeScale = 0;
        StartCoroutine(LoadGameCoroutine());
    }

    public IEnumerator LoadGameCoroutine()
    {
        PlayerData.AllowGameInput(false);
        yield return FaderController.instance.FadeOut();
        FaderController.instance.setOpaque();
        SaveSystem.Load(PlayerData.saveIndex);
        startScene = PlayerData.currentScene;
        yield return SceneManager.UnloadSceneAsync("Title");

        yield return SceneManager.LoadSceneAsync(persistentGame);


        if (!SceneManager.GetSceneByName(startScene).isLoaded)
            yield return SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);

        
        
        
        PlayerData.AllowGameInput(false);
        Time.timeScale = 1;
        yield return FaderController.instance.FadeIn();
        PlayerData.AllowGameInput(true);
        //Destroy(this.gameObject);
    }

    public IEnumerator LoadGameFromPlayerDeath()
    {
        PlayerData.AllowGameInput(false);
        yield return FaderController.instance.FadeOut();
        FaderController.instance.setOpaque();
        SaveSystem.Load(PlayerData.saveIndex);
        startScene = PlayerData.currentScene;


        yield return SceneManager.LoadSceneAsync(persistentGame, LoadSceneMode.Single);

        //if (!SceneManager.GetSceneByName(startScene).isLoaded)
        yield return SceneManager.LoadSceneAsync(startScene, LoadSceneMode.Additive);

        
        

        PlayerData.AllowGameInput(false);
        Time.timeScale = 1;
        yield return FaderController.instance.FadeIn();
        PlayerData.AllowGameInput(true);
    }

    //private IEnumerator Start()
    //{
    //    yield return fader.FadeIn();
    //}
}
