using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIButtonFader : MonoBehaviour
{

    public GameObject[] imageButtonsToFadeIn;

    public GameObject[] imageButtonsToFadeOut;

    public GameObject[] textButtonsToFadeIn;

    public GameObject[] textButtonsToFadeOut;

    private List<Image> imagesToFadeIn = new();
    private List<Image> imagesToFadeOut = new();

    private List<TextMeshProUGUI> textToFadeIn = new();
    private List<TextMeshProUGUI> textToFadeOut = new();

    private List<Button> buttonsToEnable = new();
    private List<Button> buttonsToDisable = new();

    public float fadeDurationSeconds = 1.0f;

    private void Awake()
    {

        if (imageButtonsToFadeIn != null)
        {
            for (int i = 0; i < imageButtonsToFadeIn.Length; i++)
            {
                if (imageButtonsToFadeIn[i].gameObject.activeInHierarchy)
                {
                    imagesToFadeIn.Add(imageButtonsToFadeIn[i].GetComponent<Image>());
                    if (imageButtonsToFadeIn[i].GetComponent<Button>() != null) 
                        buttonsToEnable.Add(imageButtonsToFadeIn[i].GetComponent<Button>());
                } else
                {
                    imageButtonsToFadeIn[i].SetActive(true);
                    imagesToFadeIn.Add(imageButtonsToFadeIn[i].GetComponent<Image>());
                    if (imageButtonsToFadeIn[i].GetComponent<Button>() != null)
                        buttonsToEnable.Add(imageButtonsToFadeIn[i].GetComponent<Button>());
                    imageButtonsToFadeIn[i].SetActive(false);
                }


                
            }
        }

        if (imageButtonsToFadeOut != null)
        {
            for (int i = 0; i < imageButtonsToFadeOut.Length; i++)
            {

                if (imageButtonsToFadeOut[i].gameObject.activeInHierarchy)
                {

                    imagesToFadeOut.Add(imageButtonsToFadeOut[i].GetComponent<Image>());
                    if (imageButtonsToFadeOut[i].GetComponent<Button>() != null)
                        buttonsToDisable.Add(imageButtonsToFadeOut[i].GetComponent<Button>());
                } else
                {
                    imageButtonsToFadeOut[i].SetActive(true);
                    imagesToFadeOut.Add(imageButtonsToFadeOut[i].GetComponent<Image>());
                    if (imageButtonsToFadeOut[i].GetComponent<Button>() != null)
                        buttonsToDisable.Add(imageButtonsToFadeOut[i].GetComponent<Button>());
                    imageButtonsToFadeOut[i].SetActive(false);
                }
            }
        }


        if (textButtonsToFadeIn != null)
        {
            for (int i = 0; i < textButtonsToFadeIn.Length; i++)
            {

                if (textButtonsToFadeIn[i].activeInHierarchy)
                {
                    textToFadeIn.Add(textButtonsToFadeIn[i].GetComponent<TextMeshProUGUI>());
                    if (textButtonsToFadeIn[i].GetComponent<Button>() != null)
                        buttonsToEnable.Add(textButtonsToFadeIn[i].GetComponent<Button>());
                }
                else
                {
                    textButtonsToFadeIn[i].SetActive(true);
                    textToFadeIn.Add(textButtonsToFadeIn[i].GetComponent<TextMeshProUGUI>());
                    if (textButtonsToFadeIn[i].GetComponent<Button>() != null)
                        buttonsToEnable.Add(textButtonsToFadeIn[i].GetComponent<Button>());
                    textButtonsToFadeIn[i].SetActive(false);
                }
            }
        }

        if (textButtonsToFadeOut != null)
        {
            for (int i = 0; i < textButtonsToFadeOut.Length; i++)
            {
                if (textButtonsToFadeOut[i].activeInHierarchy)
                {
                    textToFadeOut.Add(textButtonsToFadeOut[i].GetComponent<TextMeshProUGUI>());
                    if (textButtonsToFadeOut[i].GetComponent<Button>() != null)
                        buttonsToDisable.Add(textButtonsToFadeOut[i].GetComponent<Button>());
                }
                else
                {
                    textButtonsToFadeOut[i].SetActive(true);
                    textToFadeOut.Add(textButtonsToFadeOut[i].GetComponent<TextMeshProUGUI>());
                    if (textButtonsToFadeOut[i].GetComponent<Button>() != null)
                        buttonsToDisable.Add(textButtonsToFadeOut[i].GetComponent<Button>());
                    textButtonsToFadeOut[i].SetActive(false);
                }
            }
        }
    }

    public void FadeInButtons()
    {
        StartCoroutine(FadeInRoutine());
        
    }

    public void FadeOutButtons()
    {
        StartCoroutine(FadeOutRoutine());
    }

    public void FadeOutThenFadeIn(float secondsBeforeFadeIn)
    {
        StartCoroutine(FadeOutThenFadeInCoroutine(secondsBeforeFadeIn));
    }

    private IEnumerator FadeOutThenFadeInCoroutine(float secondsBeforeFadeIn)
    {
        yield return FadeOutRoutine();
        yield return new WaitForSeconds(secondsBeforeFadeIn);
        yield return FadeInRoutine();
    }


    private IEnumerator FadeInRoutine()
    {

        for (int i = 0; i < buttonsToEnable.Count; i++)
        {
            buttonsToEnable[i].gameObject.SetActive(true);
            buttonsToEnable[i].interactable = false;

        }


        float startAlpha = 0f;
        float endAlpha = 1f;
        float elapsedTime = 0f;

        


        while (elapsedTime < fadeDurationSeconds)
        {
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDurationSeconds);

            for (int i = 0; i < imagesToFadeIn.Count; i++)
            {

                imagesToFadeIn[i].color = Color.Lerp(new Color(imagesToFadeIn[i].color.r, imagesToFadeIn[i].color.g, imagesToFadeIn[i].color.b, startAlpha),
                new Color(imagesToFadeIn[i].color.r, imagesToFadeIn[i].color.g, imagesToFadeIn[i].color.b, endAlpha), t);
                
            }

            for (int i = 0; i < textToFadeIn.Count; i++)
            {

                textToFadeIn[i].color = Color.Lerp(new Color(textToFadeIn[i].color.r, textToFadeIn[i].color.g, textToFadeIn[i].color.b, startAlpha),
                new Color(textToFadeIn[i].color.r, textToFadeIn[i].color.g, textToFadeIn[i].color.b, endAlpha), t);

            }


            yield return null;
            
        }



        for (int i = 0; i < buttonsToEnable.Count; i++)
        {
            buttonsToEnable[i].interactable = true;
        }

        //fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, endAlpha);
    }

    private IEnumerator FadeOutRoutine()
    {

        for (int i = 0; i < buttonsToDisable.Count; i++)
        {
            buttonsToDisable[i].gameObject.SetActive(true);
            buttonsToDisable[i].interactable = false;
            

        }


        float startAlpha = 1f;
        float endAlpha = 0f;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDurationSeconds)
        {
            
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDurationSeconds);

            for (int i = 0; i < imagesToFadeOut.Count; i++)
            {

                imagesToFadeOut[i].color = Color.Lerp(new Color(imagesToFadeOut[i].color.r, imagesToFadeOut[i].color.g, imagesToFadeOut[i].color.b, startAlpha),
                new Color(imagesToFadeOut[i].color.r, imagesToFadeOut[i].color.g, imagesToFadeOut[i].color.b, endAlpha), t);

            }

            for (int i = 0; i < textToFadeOut.Count; i++)
            {

                textToFadeOut[i].color = Color.Lerp(new Color(textToFadeOut[i].color.r, textToFadeOut[i].color.g, textToFadeOut[i].color.b, startAlpha),
                new Color(textToFadeOut[i].color.r, textToFadeOut[i].color.g, textToFadeOut[i].color.b, endAlpha), t);

            }


            yield return null;
            
        }

        /*for (int i = 0; i < buttonsToDisable.Count; i++)
        {
            buttonsToDisable[i].gameObject.SetActive(false);

        }*/


        //fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, endAlpha);
    }




}
