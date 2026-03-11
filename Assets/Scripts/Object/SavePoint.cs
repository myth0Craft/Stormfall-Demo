using UnityEngine;

public class SavePoint : MonoBehaviour
{
    
    private DisplaySaveIcon saveIconConrtoller;

    private void Awake()
    {
        saveIconConrtoller = GameObject.FindGameObjectWithTag("SaveIconController").GetComponent<DisplaySaveIcon>();
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            SaveSystem.Save(PlayerData.saveIndex);
            //StartCoroutine(saveIconConrtoller.DisplaySaveIconCoroutine());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            PlayerData.currentScene = gameObject.scene.name;
            PlayerData.posX = gameObject.transform.position.x;
            PlayerData.posY = gameObject.transform.position.y;
            SaveSystem.Save(PlayerData.saveIndex);
            StartCoroutine(saveIconConrtoller.DisplaySaveIconCoroutine());
        }
    }
}