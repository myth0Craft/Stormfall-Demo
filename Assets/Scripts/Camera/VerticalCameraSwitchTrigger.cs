using Unity.Cinemachine;
using UnityEngine;

public class VerticalCameraSwitchTrigger : MonoBehaviour
{

    [SerializeField] private CinemachineCamera topCam;
    [SerializeField] private CinemachineCamera bottomCam;
    private BoxCollider2D collider;

    private void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Vector2 exitDirection = (collision.transform.position - collider.bounds.center).normalized;
            if (bottomCam != null && topCam != null)
            {
                if (exitDirection.y > 0f)
                {
                    bottomCam.Priority = 0;
                    topCam.Priority = 10;


                }
                else if (exitDirection.y < 0f)
                {
                    bottomCam.Priority = 10;
                    topCam.Priority = 0;

                }
            } else
            {
                if (exitDirection.y > 0f && bottomCam != null)
                    bottomCam.Priority = 0;
                else if (exitDirection.y > 0f && topCam != null)
                    topCam.Priority = 10;
                else if (exitDirection.y < 0f && bottomCam != null)
                    bottomCam.Priority = 10;
                else if (exitDirection.y < 0f && topCam != null)
                    topCam.Priority = 0;
            }
        }
    }
}
