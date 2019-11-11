using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWithTemperatureAndFuel : MonoBehaviour
{

    public float igniteTemperature = 50f;
    public float starterFuel = 500f;

    public float minimumFuelBurn = 5f;
    public float fuelBurnRate = 10f;
    public float temperatureGainedPerFuelUnit = 2f;

    public float temperatureLossRate = 1f;

    public float maxFireSizeMultiplier = 2;
    public float spreadTemperature = 3f;

    public float currentTemperature;
    public float currentFuel;
    public bool isAlight = false;

    private FireSystemManager fireManager;

    protected Material thisMaterial;
    protected ParticleSystem[] fireParticleSystems;
    protected float[] particleStartSize;
    protected Light fireLight;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        fireManager = FindObjectOfType<FireSystemManager>();
        fireParticleSystems = GetComponentsInChildren<ParticleSystem>();
        particleStartSize = new float[fireParticleSystems.Length];
        for(int i=0; i<fireParticleSystems.Length; i++)
        {
            particleStartSize[i] = fireParticleSystems[i].main.startSize.constant;
        }
        fireLight = GetComponentInChildren<Light>();

        currentFuel = starterFuel;
    }

    protected virtual void Update()
    {
        if(isAlight)
        {
            SpendFuel();
            if(currentFuel <= 0f)
            {
                Extinguish();
            }
            AdjustParticleSize();
        }
        else
        {
            if(currentTemperature >= igniteTemperature && currentFuel > 0)
            {
                StartFire();
            }
        }

    }

    protected virtual void OnTriggerStay(Collider other)
    {
        if (isAlight)
        {
            if (fireManager.GetIfFlammable(other.GetComponentInParent<Renderer>().sharedMaterial))
            {
                if (!other.GetComponentInChildren<FireWithTemperatureAndFuel>())
                {
                    fireManager.AddFireSystem(other.gameObject);
                }
                else if (!other.GetComponent<MeshCollider>())
                {
                    SpreadTemperature(other.GetComponentInChildren<FireWithTemperatureAndFuel>());
                }
            }
        }
    }

    protected virtual void SpendFuel()
    {
        float fuelBurned = Mathf.Lerp(minimumFuelBurn, fuelBurnRate, currentFuel/starterFuel);
        if (currentFuel < fuelBurned)
        {
            fuelBurned = currentFuel;
        }
        currentFuel -= fuelBurned;
        currentTemperature += fuelBurned * temperatureGainedPerFuelUnit;
    }

    protected virtual void RadiateHeat()
    {
        currentTemperature -= temperatureLossRate;
    }

    public virtual void StartFire()
    {
        isAlight = true;
        fireLight.enabled = true;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Play();
        }
        SpendFuel();
    }

    protected virtual void Extinguish()
    {
        isAlight = false;
        fireLight.enabled = false;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Stop();
        }
    }

    protected virtual void AdjustParticleSize()
    {
        for(int i=0;i<fireParticleSystems.Length;i++)
        {
            var main = fireParticleSystems[i].main;
            main.startSize = particleStartSize[i] * Mathf.Lerp(0, maxFireSizeMultiplier, 1 - Mathf.Pow((currentFuel / starterFuel - 0.5f) * 2, 2f));
        }
    }

    public virtual void Ignite()
    {
        currentTemperature = igniteTemperature;
    }

    public bool GetIsAlight()
    {
        return isAlight;
    }

    public void SpreadTemperature(FireWithTemperatureAndFuel other)
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
