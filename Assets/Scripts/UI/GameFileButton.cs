using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameFileButton : MonoBehaviour
{

    public int saveIndex;
    private TextMeshProUGUI text;
    private Image deleteIconImage;

    public GameObject deleteButton;

    private float deleteAlpha;

    public void Awake()
    {
        deleteIconImage = deleteButton.GetComponent<Image>();
        text = GetComponent<TextMeshProUGUI>();
        deleteAlpha = deleteIconImage.color.a;
    }

    public void Update()
    {


        bool fileExists = SaveSystem.CheckIfFileExists(saveIndex);

        if (fileExists)
        {
            text.text = "Save " + (saveIndex + 1);
            deleteButton.SetActive(true);
            deleteIconImage.color = new Color(deleteIconImage.color.r, deleteIconImage.color.g, deleteIconImage.color.b, text.color.a);
        }
        else
        {
            text.text = "New Game";
            deleteButton.SetActive(false);
        }

        /*//Manually set the color from here because the UIButtonFader will not have access to save file state data.
        if (deleteIconImage.color.a != deleteAlpha)
        {
            deleteIconImage.color = new Color (deleteIconImage.color.r, deleteIconImage.color.g, deleteIconImage.color.b, text.color.a);
        }*/
    }
}
