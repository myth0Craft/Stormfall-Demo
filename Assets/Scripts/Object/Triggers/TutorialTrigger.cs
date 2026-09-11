using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private Tutorial tutorial;

    private void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.CompareTag("Player"))
        {
            if (PlayerData.completedTutorials.Contains(tutorial.tutorialID)) return;

            TutorialUIController.instance.PlayTutorial(tutorial);
        }
    }
}