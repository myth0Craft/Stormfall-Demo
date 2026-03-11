using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadSceneAsync : MonoBehaviour
{
    [SerializeField] private string[] scenesToLoad;
    [SerializeField] private string[] scenesToUnload;

    private GameObject player;
    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            LoadScenes();
            UnloadScenes();
        }

    }

    private void LoadScenes()
    {
        for (int i = 0; i < scenesToLoad.Length; i++)
        {
            bool isSceneLoaded = false;
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToLoad[i])
                {
                    isSceneLoaded = true;
                    break;
                }
            }

            if (!isSceneLoaded)
            {
                SceneManager.LoadSceneAsync(scenesToLoad[i], LoadSceneMode.Additive);
            }
        }
        //foreach (string sceneName in scenesToLoad)
        //{
        //    if (!SceneManager.GetSceneByName(sceneName).isLoaded)
        //    {
        //        SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
        //    }
        //}


    }

    private void UnloadScenes()
    {
        for (int i = 0; i < scenesToUnload.Length; i++)
        {
            for (int j = 0; j < SceneManager.sceneCount; j++)
            {
                Scene loadedScene = SceneManager.GetSceneAt(j);
                if (loadedScene.name == scenesToUnload[i])
                {
                    SceneManager.UnloadSceneAsync(scenesToUnload[i]);
                }
            }
        }

        //foreach (string sceneName in scenesToUnload)
        //{
        //    Scene scene = SceneManager.GetSceneByName(sceneName);
        //    if (scene.isLoaded)
        //    {
        //        SceneManager.UnloadSceneAsync(sceneName);
        //    }
        //}
    }
}
