using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystem : MonoBehaviour
{

    public float igniteTemperature = 50f;
    public float currentTemperature;

    public float lifeSpanSeconds = 10f;
    public float lifeRemaining;

    public float maxFireSizeMultiplier = 2;
    public float spreadTemperature = 3f;
    public bool isAlight = false;

    private FireSystemManager fireManager;

    private Material thisMaterial;
    private ParticleSystem[] fireParticleSystems;
    private float[] particleStartSize;
    private Light fireLight;

    public Vector3 particleDirection;

    // Start is called before the first frame update
    private void Start()
    {
        fireManager = FindObjectOfType<FireSystemManager>();
        fireParticleSystems = GetComponentsInChildren<ParticleSystem>();
        particleStartSize = new float[fireParticleSystems.Length];
        for(int i=0; i<fireParticleSystems.Length; i++)
        {
            particleStartSize[i] = fireParticleSystems[i].main.startSize.constant;
        }
        fireLight = GetComponentInChildren<Light>();
        AdjustParticleDirection(particleDirection);
        currentTemperature = 0f;
        lifeRemaining = lifeSpanSeconds;
    }

    private void Update()
    {
        if(isAlight)
        {
            CountDownLife();
            if(lifeRemaining <= 0f)
            {
                Extinguish();
            }
            AdjustParticleSize();
        }
        else
        {
            if(currentTemperature >= igniteTemperature && lifeRemaining > 0)
            {
                StartFire();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAlight)
        {
            if (fireManager.GetIfFlammable(other.GetComponentInParent<Renderer>().sharedMaterial))
            {
                if (!other.GetComponentInChildren<FireSystem>())
                {
                    fireManager.AddFireSystem(other.gameObject);
                }
                else if (!other.GetComponent<MeshCollider>())
                {
                    SpreadTemperature(other.GetComponentInChildren<FireSystem>());
                }
            }
        }
    }

    private void CountDownLife()
    {
        lifeRemaining -= Time.deltaTime;
    }

    private void StartFire()
    {
        isAlight = true;
        fireLight.enabled = true;
        gameObject.layer = 9;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Play();
        }
        CountDownLife();
    }

    private void Extinguish()
    {
        isAlight = false;
        fireLight.enabled = false;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Stop();
        }
        fireManager.RemoveFireSystem(this);
    }

    private void AdjustParticleSize()
    {
        for(int i=0;i<fireParticleSystems.Length;i++)
        {
            var main = fireParticleSystems[i].main;
            main.startSize = particleStartSize[i] * Mathf.Lerp(0, maxFireSizeMultiplier, 1 - Mathf.Pow((lifeRemaining / lifeSpanSeconds - 0.5f) * 2, 2f));
        }
    }

    public void AdjustParticleDirection(Vector3 newVel)
    {
        particleDirection = newVel;
        for (int i = 0; i < fireParticleSystems.Length; i++)
        {
            var vel = fireParticleSystems[i].velocityOverLifetime;
            vel.x = newVel.x;
            vel.z = newVel.z;
        }
        GetComponent<SphereCollider>().center = newVel/10;
    }

    public void Ignite()
    {
        currentTemperature = igniteTemperature;
    }

    public bool GetIsAlight()
    {
        return isAlight;
    }

    public void SpreadTemperature(FireSystem other)
    {
        other.AddTemperature(spreadTemperature);
    }

    public void AddTemperature(float addTemp)
    {
        currentTemperature += addTemp;
    }

    public void SetTemperature(float newTemp)
    {
        currentTemperature = newTemp;
    }
}
