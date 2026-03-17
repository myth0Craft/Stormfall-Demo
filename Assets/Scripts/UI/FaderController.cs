using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FaderController : MonoBehaviour
{

    public static FaderController instance { get; private set; }
    private Image fadeImage;
    [SerializeField] public float fadeDuration = 1f;
    private void Awake()
    {
        fadeImage = GetComponent<Image>();
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public IEnumerator FadeIn()
    {
        instance.StopAllCoroutines();
        //StopCoroutine("FadeRoutine");
        yield return FadeRoutine(1, 0, new Color(0, 0, 0));
        //gameObject.SetActive(false);
    }

    public IEnumerator FadeOut()
    {
        instance.StopAllCoroutines();
        //StopCoroutine("FadeRoutine");
        //gameObject.SetActive(true);
        yield return FadeRoutine(0, 1, new Color(0, 0, 0));
        
    }

    public IEnumerator FadeFromWhite()
    {
        instance.StopAllCoroutines();
        //StopCoroutine("FadeRoutine");
        yield return FadeRoutine(1, 0, new Color(255, 255, 255));
        //gameObject.SetActive(false);
    }

    public IEnumerator FadeToWhite()
    {
        instance.StopAllCoroutines();
        //StopCoroutine("FadeRoutine");
        //gameObject.SetActive(true);
        yield return FadeRoutine(0, 1, new Color(255, 255, 255));

    }

    private IEnumerator FadeRoutine(float startAlpha, float endAlpha, Color color)
    {
        fadeImage.color = color;
        float elapsedPercentage = 0f;
        float elapsedTime = 0f;
        while (elapsedPercentage < 1)
        {
            elapsedPercentage = elapsedTime / fadeDuration;
            fadeImage.color = Color.Lerp(new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, startAlpha),
                new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, endAlpha), elapsedPercentage);
            yield return null;
            elapsedTime += Time.unscaledDeltaTime;
        }

        //fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, endAlpha);
    }



    public void setOpaque()
    {
        fadeImage.color = new Color(fadeImage.color.r, fadeImage.color.g, fadeImage.color.b, 255);
    }
}
