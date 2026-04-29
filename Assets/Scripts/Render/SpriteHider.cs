using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SpriteHider : MonoBehaviour
{
    public List<SpriteRenderer> spritesToHide = new List<SpriteRenderer>();
    public float fadeDuration = 0.5f;
    public float transparency = 0.2f;
    public bool foregroundSprites = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(HideSpritesCoroutine(1.0f, transparency));
        }        
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Application.isPlaying && gameObject.activeSelf)
        {
            StartCoroutine(HideSpritesCoroutine(transparency, 1.0f));
        }
    }

    private IEnumerator HideSpritesCoroutine(float startAlpha, float endAlpha)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            foreach (SpriteRenderer sprite in spritesToHide)
            {
                if (sprite != null)
                {
                    sprite.color = Color.Lerp(new Color(sprite.color.r, sprite.color.g, sprite.color.b, startAlpha), new Color(sprite.color.r, sprite.color.g, sprite.color.b, endAlpha), t);
                }
            }
            
            if (foregroundSprites)
            {
                if (endAlpha < startAlpha)
                {
                    foreach (SpriteRenderer sprite in spritesToHide)
                    {
                        sprite.gameObject.layer = LayerMask.NameToLayer("Default");
                    }

                } else
                {
                    foreach (SpriteRenderer sprite in spritesToHide)
                    {
                        sprite.gameObject.layer = LayerMask.NameToLayer("Foreground");
                    }
                }
                
            }
            
            
            yield return null;
        }

        foreach (SpriteRenderer sprite in spritesToHide)
        {
            if (sprite != null)
            {
                sprite.color = new Color(sprite.color.r, sprite.color.g, sprite.color.b, endAlpha);
            }
        }
    }

}
