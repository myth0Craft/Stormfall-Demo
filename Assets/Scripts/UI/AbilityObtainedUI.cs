using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class AbilityObtainedUI : MonoBehaviour
{

    public static AbilityObtainedUI instance;

    private Image abilityImage;
    private Image inputHintImage;
    private TextMeshProUGUI abilityText;
    private TextMeshProUGUI abilitySubText;
    private TextMeshProUGUI abilityDescriptor;
    private Image overlay;

    private TextMeshProUGUI continueText;
    private Image continueIcon;

    public Sprite[] abilityIconSprites;
    public Sprite[] inputIconSprites;
    

    //private InputAction anyButtonAction;
    private bool anyButtonPressed;


    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        PlayerControls controls = PlayerData.getControls();
        controls.Player.Interact.performed += ctx => anyButtonPressed = true;
        controls.Player.Jump.performed += ctx => anyButtonPressed = true;
        controls.Player.Dash.performed += ctx => anyButtonPressed = true;

        abilityImage = transform.Find("AbilityImage").GetComponent<Image>();

        inputHintImage = transform.Find("InputHintIcon").GetComponent<Image>();

        abilityText = transform.Find("AbilityText").GetComponent <TextMeshProUGUI>();
        abilitySubText = transform.Find("AbilitySubText").GetComponent<TextMeshProUGUI>();
        abilityDescriptor = transform.Find("AbilityDescription").GetComponent<TextMeshProUGUI>();
        overlay = transform.Find("Overlay").GetComponent<Image>();

        continueText = transform.Find("Continue").GetComponent<TextMeshProUGUI>();
        continueIcon = transform.Find("ContinueIcon").GetComponent<Image>();

        if (abilityImage == null || inputHintImage == null || abilityText == null || abilitySubText == null || abilityDescriptor == null || overlay == null || continueText == null || continueIcon == null)
        {
            Debug.Log("Ability UI not configured correctly! Ensure no components are missing and child game object names are set correctly!");
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

    }



    public IEnumerator FadeInAbilityScreen(int abilityIndex, int inputIconIndex, string abilityName, string abilitySubText, string abilityDesc)
    {
        FaderController.instance.setOpaque();

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        if (abilityIconSprites[abilityIndex] == null)
        {
            Debug.Log("Invalid ability index");
        } else
        {
            abilityImage.sprite = abilityIconSprites[abilityIndex];
        }

        if (inputIconSprites[inputIconIndex] == null)
        {
            Debug.Log("Invalid input icon index");
        }
        else
        {
            inputHintImage.sprite = inputIconSprites[inputIconIndex];
        }

        abilityText.text = abilityName;
        this.abilitySubText.text = abilitySubText;
        abilityDescriptor.text = abilityDesc;
        yield return FadeInAbilityScreenCoroutine(1.0f);
        
    }

    private IEnumerator FadeInAbilityScreenCoroutine(float fadeDuration)
    {
        FaderController.instance.fadeDuration = fadeDuration;
        yield return FaderController.instance.FadeFromWhite();
        anyButtonPressed = false;
        yield return new WaitForSecondsRealtime(2.5f);

        
        while (anyButtonPressed == false)
        {
            yield return null;
        }



        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
    }
}
