using Unity.Cinemachine;
using UnityEngine;

public class CameraLoader : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cam != null)
        {
            if (collision.CompareTag("Player"))
            {
                cam.enabled = true;
                cam.Priority = 10;
            }
                
        }
    }
}
