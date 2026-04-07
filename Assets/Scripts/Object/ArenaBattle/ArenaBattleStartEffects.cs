using System.Collections;
using UnityEngine;

[System.Serializable]
public class ArenaBattleStartEffects : MonoBehaviour
{
    public float cameraShakeStrength = 0f;
    public GameObject particles;
    //public Vector2 particleWorldPos;
    public float effectsDurationSeconds = 2.0f;

    public IEnumerator AddArenaBattleStartEffects()
    {
        GameObject particle = Instantiate(
            particles,
            transform.position,
            Quaternion.identity
        );
        ParticleSystem particleSystem = particle.GetComponent<ParticleSystem>();
        particleSystem.Play();
        if (cameraShakeStrength > 0f)
        {
            ContinuousCameraShakeSource.instance.AddScreenShakeOverTime(cameraShakeStrength, effectsDurationSeconds, 0.2f);
        }

        yield return new WaitForSecondsRealtime(effectsDurationSeconds);
        particleSystem.Stop();
        /*yield return new WaitForSecondsRealtime(5f);
        Destroy(particle);*/
        
    }


}
