using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class AutosaveNoticeController : MonoBehaviour
{
    private CanvasGroupFader fader;
    [SerializeField] private CanvasGroup blackPanel;

    private void Awake()
    {
        fader = GetComponent<CanvasGroupFader>();
        blackPanel.gameObject.SetActive(true);
        StartCoroutine(AutosaveNoticeCoroutine());
    }

    private IEnumerator AutosaveNoticeCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        fader.FadeIn();
        yield return new WaitForSecondsRealtime(3.5f);
        fader.FadeOut();
        yield return new WaitForSecondsRealtime(1.1f);
        fader.canvasGroupsToFadeOut = new List<CanvasGroup>() { blackPanel };
        fader.FadeOut();
    }
}
