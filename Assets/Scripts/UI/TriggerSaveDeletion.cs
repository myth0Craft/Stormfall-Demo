using UnityEngine;

public class TriggerSaveDeletion : MonoBehaviour
{
    public int saveIndex = 0;

    public void TriggerDeleteSaveFile()
    {
        SaveSystem.ResetSaveFile(saveIndex);
    }
}
