using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DisplaySaveIcon : MonoBehaviour
{

    public static DisplaySaveIcon Instance { get; private set; }

    private Image saveIcon;
    private float saveIconFadeDuration = 0.5f;

    private bool coroutineActive = false;

    private void Awake()
    {
        if (Instance == null || Instance != this)
            Instance = this;


        
    }

    private void Start()
    {
        saveIcon = GameObject.FindGameObjectWithTag("SaveIcon").GetComponent<Image>();
    }

    public IEnumerator DisplaySaveIconCoroutine()
    {

        if (coroutineActive)
        {
            StopCoroutine(DisplaySaveIconCoroutine());
            StartCoroutine(DisplaySaveIconCoroutine());
        }
        coroutineActive = true;

        saveIcon.color = new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 0f);
        saveIcon.enabled = true;


        float elapsedPercentage = 0f;
        float elapsedTime = 0f;
        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / saveIconFadeDuration;
            saveIcon.color = Color.Lerp(new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 0),
                new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 1), elapsedPercentage);
            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
        }



        yield return new WaitForSecondsRealtime(1.0f);


        elapsedPercentage = 0f;
        elapsedTime = 0f;
        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / saveIconFadeDuration;
            saveIcon.color = Color.Lerp(new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 1),
                new Color(saveIcon.color.r, saveIcon.color.g, saveIcon.color.b, 0), elapsedPercentage);
            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
        }


        saveIcon.enabled = false;
        coroutineActive = false;
    }

}
