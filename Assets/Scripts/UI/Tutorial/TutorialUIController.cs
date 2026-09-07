using NUnit.Framework.Internal;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class TutorialUIController : MonoBehaviour
{
    private CanvasGroupFader fader;

    public float duration = 0.5f;

    [SerializeField] private Vector2 vignetteCenteredOnStaminaCoordinates;

    [SerializeField] private Volume volumeOverlay;
    private Vignette vignette;
    [SerializeField] private float startVignetteIntensity;

    [SerializeField] private CanvasGroup staminaFocusedCanvasGroup;
    [SerializeField] private TextMeshProUGUI staminaText;

    [SerializeField] private CanvasGroup centeredCanvasGroup;
    [SerializeField] private TextMeshProUGUI centeredText;
    

    public static TutorialUIController instance;

    private Coroutine currentTutorial;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        } else
        {
            Destroy(this);
        }
        fader = GetComponent<CanvasGroupFader>();
    }

    private IEnumerator FadeInVolumeOverlay(Vector2 vignetteCenter)
    {
        

        volumeOverlay.gameObject.SetActive(true);

        if (volumeOverlay.profile.TryGet(out vignette))
        {
            vignette.center.overrideState = true;
            vignette.center.value = vignetteCenter;

            vignette.intensity.overrideState = true;
            vignette.intensity.value = startVignetteIntensity;

            float elapsedTime = 0;

            while (elapsedTime < duration)
            {
                vignette.intensity.value = Mathf.Min((float)((elapsedTime * 0.4) / duration) + startVignetteIntensity, 0.4f);



                elapsedTime += Time.unscaledDeltaTime;
                yield return null;
            }

            vignette.intensity.value = 0.4f;
        }
    }

    private IEnumerator FadeOutVolumeOverlay()
    {
        if (volumeOverlay.profile.TryGet(out vignette))
        {

            vignette.intensity.overrideState = true;
            vignette.intensity.value = 0.4f;

            float elapsedTime = duration;

            while (elapsedTime > 0)
            {
                vignette.intensity.value = Mathf.Max((float)((elapsedTime * 0.4) / duration), startVignetteIntensity);

                elapsedTime -= Time.unscaledDeltaTime;
                yield return null;
            }

            vignette.intensity.value = startVignetteIntensity;
        }

        volumeOverlay.gameObject.SetActive(false);
    }


    private void FadeInTutorial(CanvasGroup groupToFadeIn)
    {
        fader.canvasGroupsToFadeIn = new System.Collections.Generic.List<CanvasGroup> { groupToFadeIn };
        fader.fadeDuration = duration;
        fader.FadeIn();
    }

    private void FadeOutTutorial(CanvasGroup groupToFadeOut)
    {
        fader.canvasGroupsToFadeOut = new System.Collections.Generic.List<CanvasGroup> { groupToFadeOut };
        fader.fadeDuration = duration;
        fader.FadeOut();
    }

    public void PlayTutorial(Tutorial tutorial)
    {

        if (currentTutorial != null)
        {
            StopCoroutine(currentTutorial);
        }

        currentTutorial = StartCoroutine(PlayTutorialCoroutine(tutorial));
    }

    private IEnumerator PlayTutorialCoroutine(Tutorial tutorial)
    {

        PlayerData.AllowGameInput(false);

        Vector2 vignetteCenter;

        float previousTimeScale = Time.timeScale;

        if (tutorial.pauseGame)
        {
            Time.timeScale = 0f;
        }

        if (tutorial.focusedOnStamina)
        {
            vignetteCenter = vignetteCenteredOnStaminaCoordinates;
        } else
        {
            //player screen space pos
            vignetteCenter = Camera.main.WorldToScreenPoint(PlayerMovement.instance.gameObject.transform.position);
            vignetteCenter.x = vignetteCenter.x / Screen.width;
            vignetteCenter.y = vignetteCenter.y / Screen.height;
        }

        TextMeshProUGUI text;

        CanvasGroup groupToFadeIn;

        if (tutorial.focusedOnStamina)
        {
            text = staminaText;
            groupToFadeIn = staminaFocusedCanvasGroup;
        } else
        {
            text = centeredText;
            groupToFadeIn = centeredCanvasGroup;
        }

        text.text = tutorial.text;

        FadeInTutorial(groupToFadeIn);

        yield return FadeInVolumeOverlay(vignetteCenter);

        tutorial.condition.StartListening();
        while(!tutorial.condition.IsComplete())
        {
            yield return null;
        }
        tutorial.condition.StopListening();

        PlayerData.AllowGameInput(true);

        Time.timeScale = previousTimeScale;

        FadeOutTutorial(groupToFadeIn);

        yield return FadeOutVolumeOverlay();

        PlayerData.MarkCompletedTutorial(tutorial.tutorialID);
        SaveSystem.Save(PlayerData.saveIndex);
        DisplaySaveIcon.Instance.DisplaySaveIconCoroutine();
    }
}