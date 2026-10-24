using UnityEngine;

public class BetaOnlyObject : MonoBehaviour
{
    public bool enabledInBeta = false;

    void Update()
    {
        if (enabledInBeta)
        {
            if (PlayerData.betaFeaturesEnabled)
            {
                SetChildrenActive(true);
            }
            else
            {
                SetChildrenActive(false);
            }
        }
        else
        {
            if (PlayerData.betaFeaturesEnabled)
            {
                SetChildrenActive(false);
            }
            else
            {
                SetChildrenActive(true);
            }
        }

    }

    public void SetChildrenActive(bool active)
    {
        foreach (Transform t in transform)
        {
            t.gameObject.SetActive(active);
        }
    }
}
