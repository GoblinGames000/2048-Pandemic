using UnityEngine;

public class ParticleSelfDestroy : MonoBehaviour
{
    private ParticleSystem particles;

    void Awake()
    {
        this.useGUILayout = false;
        this.particles = this.GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (!this.particles.IsAlive())
        {
            Destroy(this.gameObject);
        }
    }
}
