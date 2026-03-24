using UnityEngine;

public class UIButtonEnableGameObjects : MonoBehaviour
{
    public GameObject[] gameObjectsToHide;
    public GameObject[] gameObjectsToShow;

    public void UpdateGameObjectVisibility()
    {
        for (int i = 0; i < gameObjectsToHide.Length; i++)
        {
            gameObjectsToHide[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < gameObjectsToShow.Length; i++)
        {
            gameObjectsToShow[i].gameObject.SetActive(true);
        }
    }
}
