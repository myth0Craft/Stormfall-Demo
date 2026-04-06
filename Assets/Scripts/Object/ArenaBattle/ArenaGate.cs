using System.Collections;
using UnityEngine;

public class ArenaGate : MonoBehaviour
{
    public Vector2 startPosition;
    public Vector2 endPosition;

    

    /*//Only move the sprite gameObject to prevent weird collision issues from moving the collider game object.
    public SpriteRenderer[] spriteRenderers;*/

    public float closeSpeed = 1.0f;

    private void Awake()
    {
        /*for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].gameObject.transform.position = startPosition;
        }*/

        gameObject.transform.localPosition = startPosition;

        
        gameObject.SetActive(false);
    }

    public void BeginArenaBattle()
    {
        gameObject.SetActive(true);
        StartCoroutine(MoveCoroutine(startPosition, endPosition, false));

    }

    public void EndArenaBattle()
    {
        StartCoroutine(MoveCoroutine(endPosition, startPosition, true));
    }


    private IEnumerator MoveCoroutine(Vector2 startPos, Vector2 endPos, bool disableOnEnd)
    {
        float elapsed = 0f;

        while (elapsed < closeSpeed)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / closeSpeed;

            transform.localPosition = Vector2.Lerp(startPos, endPos, t);
            yield return null;
        }
        CamShakeSource.instance.AddVerticalScreenShake(0.08f);

        transform.localPosition = endPos;

        if (disableOnEnd)
        {
            gameObject.SetActive(false);
        }
    }
}
