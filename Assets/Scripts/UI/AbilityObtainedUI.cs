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
    private TextMeshProUGUI abilityDescriptor;
    private Image overlay;

    public Sprite[] abilityIconSprites;
    public Sprite[] inputIconSprites;
    

    private InputAction anyButtonAction;
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

        anyButtonAction = new InputAction(type: InputActionType.Button, binding: "/*/<button>");

        anyButtonAction.Enable();

        anyButtonAction.performed += ctx => anyButtonPressed = true;

        abilityImage = transform.Find("AbilityImage").GetComponent<Image>();

        inputHintImage = transform.Find("InputHintIcon").GetComponent<Image>();

        abilityText = transform.Find("AbilityText").GetComponent <TextMeshProUGUI>();
        abilityDescriptor = transform.Find("AbilityDescription").GetComponent<TextMeshProUGUI>();
        overlay = transform.Find("Overlay").GetComponent<Image>();

        if (abilityImage == null || inputHintImage == null || abilityText == null || abilityDescriptor == null || overlay == null)
        {
            Debug.Log("Ability UI not configured correctly! Ensure no components are missing and child game object names are set correctly!");
        }

        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

    }



    public void FadeInAbilityScreen(int abilityIndex, int inputIconIndex, string abilityName, string abilityDesc)
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
        abilityDescriptor.text = abilityDesc;
        StartCoroutine(FadeInAbilityScreenCoroutine(1.0f));
        
    }

    public IEnumerator FadeInAbilityScreenCoroutine(float fadeDuration)
    {
        FaderController.instance.fadeDuration = fadeDuration;
        yield return FaderController.instance.FadeFromWhite();
        yield return new WaitForSecondsRealtime(3.0f);

        anyButtonPressed = false;
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
