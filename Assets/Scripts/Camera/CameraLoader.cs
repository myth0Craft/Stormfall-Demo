using Unity.Cinemachine;
using UnityEngine;

public class CameraLoader : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    private void Awake()
    {
        if (cam != null)
        {
            cam.Priority = 0;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (cam != null)
        {
            if (collision.CompareTag("Player"))
                cam.enabled = false;
            cam.enabled = true;
            cam.Priority = 10;
        }
    }
}
