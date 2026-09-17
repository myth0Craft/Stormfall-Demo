using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public static DialogueUI instance {get; private set;}

    [SerializeField] private float defaultTextYPos;
    [SerializeField] private float centeredTextYPos;

    private TextMeshProUGUI text;

    private Image background;

    private StringBuilder stringBuilder = new StringBuilder();

    public float textSpeed = 10f;
    protected PlayerControls controls;
    private bool interactPressed = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(this.gameObject);
        } else
        {
            Destroy(this.gameObject);
        }
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(true);
        }


        controls = PlayerData.getControls();
        controls.Player.ContextAction.performed += ctx => interactPressed = true;
        text = GetComponentInChildren<TextMeshProUGUI>();
        background = GetComponentInChildren<Image>();
        if (text == null || background == null)
        {
            Debug.Log("Dialogue UI is missing text or background image!");
        }
        text.enabled = false;
        background.enabled = false;
    }


    private void disableControls()
    {
        PlayerData.AllowGameInput(false);
    }

    public IEnumerator DisplayDialogueChain(List<string> dialogue)
    {
        yield return DisplayDialogueChain(dialogue, Color.white, true, false);
    }

    public void DisplayDialogue(string dialogue)
    {
        DisplayDialogue(dialogue, Color.white, false);
    }

    public void DisplayDialogue(string dialogue, Color textColor, bool centered)
    {
        if (centered)
        {
            SetTextCentered();
        } else
        {
            SetTextToDefaultYPos();
        }

        text.color = textColor;
        text.enabled = true;
        background.enabled = true;
        background.color = new Color(background.color.r, background.color.g, background.color.b, 0f);
        text.text = "";
        stringBuilder.Remove(0, stringBuilder.Length);
        Debug.Log("displaying dialogue");
        StartCoroutine(DisplayDialogueCoroutine(dialogue));
    }

    public IEnumerator DisplayDialogueChain(List<string> dialogue, Color textColor, bool displayDialogueBackground, bool centered)
    {
        text.color = textColor;
        text.enabled = true;

        if (centered)
        {
            SetTextCentered();
        }
        else
        {
            SetTextToDefaultYPos();
        }

        if (displayDialogueBackground)
        {
            background.enabled = true;
            background.color = new Color(background.color.r, background.color.g, background.color.b, 1f);
        }
        text.text = "";
        stringBuilder.Remove(0, stringBuilder.Length);


        if (displayDialogueBackground)
        {
            yield return FadeInDialogueBackgroundCoroutine(0.1f, 0f, 1f);
        }

        PlayerData.AllowGameInput(false);
        controls.Player.ContextAction.Enable();
        PlayerContextActionInputManager.instance.SetUIContext();
        for (int i = 0; i < dialogue.Count; i++)
        {
            yield return DisplayDialogueCoroutine(dialogue[i]);
            interactPressed = false;

            while (interactPressed == false)
            {
                yield return null;
            }


            text.text = "";
            stringBuilder.Remove(0, stringBuilder.Length);
        }

        PlayerContextActionInputManager.instance.RestoreContext();
        PlayerData.AllowGameInput(true);

        if (displayDialogueBackground)
        {
            yield return FadeInDialogueBackgroundCoroutine(0.1f, 1f, 0f);
        }
    }

    public IEnumerator FadeInDialogueBackgroundCoroutine(float fadeInDuration, float startAlpha, float endAlpha)
    {
        
        float elapsedTime = 0f;

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeInDuration);



            background.color = Color.Lerp(new Color(background.color.r, background.color.g, background.color.b, startAlpha),
            new Color(background.color.r, background.color.g, background.color.b, endAlpha), t);



            yield return null;

        }
    }

    private IEnumerator DisplayDialogueCoroutine(string dialogue)
    {
        PlayerContextActionInputManager.instance.SetUIContext();
        interactPressed = false;
        foreach (char c in dialogue)
        {
            if (interactPressed)
            {
                text.text = "";
                stringBuilder.Remove(0, stringBuilder.Length);
                foreach(char a in dialogue)
                {
                    stringBuilder.Append(a);
                    text.text = stringBuilder.ToString();
                }
                break;
            }
            stringBuilder.Append(c);
            text.text = stringBuilder.ToString();
            yield return new WaitForSecondsRealtime(1f / textSpeed);
        }

        PlayerContextActionInputManager.instance.RestoreContext();


    }



    public IEnumerator DisplayFullscreenDialogueCoroutine(List<string> dialogue, bool reenableInputWhenFinished)
    {
        PlayerData.AllowGameInput(false);

        if (FaderController.instance != null) {

            FaderController.instance.fadeDuration = 2.0f;

            yield return FaderController.instance.FadeFromBlackToWhite();
        }

        yield return DisplayDialogueChain(dialogue, Color.black, false, true);

        if (!reenableInputWhenFinished)
        {
            PlayerData.AllowGameInput(false);
            PlayerMovement.instance.ResetHorizontalMovement();
        }

        if (FaderController.instance != null) {

            yield return FaderController.instance.FadeFromWhite();
        }

        if (reenableInputWhenFinished)
        {
            PlayerData.AllowGameInput(true);
        }
        
    }

    private void ChangeTextPosition(float textYPos)
    {
        text.gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, textYPos);
    }

    public void SetTextCentered()
    {
        ChangeTextPosition(centeredTextYPos);
    }

    public void SetTextToDefaultYPos()
    {
        ChangeTextPosition(defaultTextYPos);
    }
}
