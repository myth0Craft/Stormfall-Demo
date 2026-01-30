using Unity.Cinemachine;
using UnityEngine;

public class CameraLoader : MonoBehaviour
{
    [SerializeField] private CinemachineCamera cam;
    private void Awake()
    {
        cam.Priority = 0;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            cam.Priority = 10;
    }
}
