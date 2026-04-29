using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PauseGame : MonoBehaviour
{
    private PlayerControls controls;
    private bool pausePressed;

    public GameObject parentObject;
    private GameObject[] childObjects;
    public GameObject[] UIToHide;
    private bool _inOptionsMenu = false;
    public Button backButton;

    private void Awake()
    {
        controls = PlayerData.getControls();
        controls.Player.Pause.performed += ctx => pausePressed = true;

        childObjects = new GameObject[parentObject.transform.childCount];

        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i] = parentObject.transform.GetChild(i).gameObject;
        }

        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i].SetActive(false);
        }
    }

    private void Update()
    {
        if (pausePressed && !PlayerData.gamePaused)
        {
            pausePressed = false;
            PlayerData.gamePaused = true;
            
            OnGamePause();
            
        }
        else if (pausePressed && PlayerData.gamePaused)
        {
            pausePressed = false;

            if (!backButton.gameObject.activeInHierarchy)
            {
                backButton.gameObject.SetActive(true);
            }
            backButton.onClick.Invoke();

            PlayerData.gamePaused = false;

            OnGameUnpause();
            
            
            

            
            
        }
    }

    public void SetInOptionsMenu(bool inOptionsMenu)
    {
        _inOptionsMenu = inOptionsMenu;
    }

    public void OnGamePause()
    {
        Time.timeScale = 0;
        PlayerData.gamePaused = true;
        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i].SetActive(true);
            if (childObjects[i].GetComponent<TextMeshProUGUI>() != null)
            {
                TextMeshProUGUI text = childObjects[i].GetComponent<TextMeshProUGUI>();
                text.color = new Color(text.color.r, text.color.g, text.color.b, 1);
            }


            /*if (childObjects[i].GetComponent<Image>() != null)
            {
                Image image = childObjects[i].GetComponent<Image>();
                image.color = new Color(image.color.r, image.color.g, image.color.b, 1);
            }*/
        }

        /*foreach(Transform t in transform)
        {
            transform.gameObject.SetActive(true);
        }*/

        for (int i = 0; i < UIToHide.Length; i++)
        {
            UIToHide[i].SetActive(false);
        }

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

    }

    public void OnGameUnpause()
    {
        
        PlayerData.gamePaused = false;
        Time.timeScale = 1;
        for (int i = 0; i < childObjects.Length; i++)
        {
            childObjects[i].SetActive(false);
        }

        for (int i = 0; i < UIToHide.Length; i++)
        {
            UIToHide[i].SetActive(true);
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
