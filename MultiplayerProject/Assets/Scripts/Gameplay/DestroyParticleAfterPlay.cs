using UnityEngine;

public class DestroyParticleAfterPlay : MonoBehaviour
{
    void Start()
    {
        ParticleSystem parts = GetComponent<ParticleSystem>();
        float totalDuration = parts.duration + parts.startLifetime;
        Destroy(gameObject, totalDuration);
    }
}
