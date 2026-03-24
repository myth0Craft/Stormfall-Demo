using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeleteSaveButton : MonoBehaviour
{
    public int saveIndex = 0;

    public GameObject deleteConfirmation;

    public GameObject saveText;

    private TriggerSaveDeletion triggerSaveDeletion;

    public void Awake()
    {


        foreach(Transform t in deleteConfirmation.transform)
        {
            t.gameObject.SetActive(true);
        }

        deleteConfirmation.SetActive(true);

        triggerSaveDeletion = deleteConfirmation.GetComponentInChildren<TriggerSaveDeletion>();

        foreach (Transform t in deleteConfirmation.transform)
        {
            t.gameObject.SetActive(false);
        }
    }

    public void ShowConfirmation()
    {
        deleteConfirmation.SetActive(true);
        
        deleteConfirmation.transform.position = new Vector3(gameObject.transform.position.x + 3, gameObject.transform.position.y, 0);

        foreach(Transform t in deleteConfirmation.transform)
        {
            t.gameObject.SetActive(true);
        }

        saveText.SetActive(false);
        

        triggerSaveDeletion.saveIndex = saveIndex;

        gameObject.SetActive(false);
    }
}
