using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireWithTemperatureAndFuel : MonoBehaviour
{

    public float igniteTemperature = 50f;
    public float starterFuel = 2000f;

    public float minimumFuelBurn = 0.1f;
    public float fuelBurnRate = 2f;
    public float temperatureGainedPerFuelUnit = 2f;

    public float temperatureLossRate = 1f;

    public float maxFireSizeMultiplier = 2;

    public float currentTemperature;
    private float currentFuel;
    private bool isAlight = false;

    private Material thisMaterial;
    private ParticleSystem[] fireParticleSystems;
    private float[] particleStartSize;
    private Light fireLight;

    // Start is called before the first frame update
    void Start()
    {
        fireParticleSystems = GetComponentsInChildren<ParticleSystem>();
        particleStartSize = new float[fireParticleSystems.Length];
        for(int i=0; i<fireParticleSystems.Length; i++)
        {
            particleStartSize[i] = fireParticleSystems[i].main.startSize.constant;
        }
        fireLight = GetComponentInChildren<Light>();

        currentFuel = starterFuel;
    }

    private void Update()
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

    private void OnTriggerStay(Collider other)
    {
        if (isAlight)
        {
            if (FindObjectOfType<FireSystemManager>())
            {
                if (FindObjectOfType<FireSystemManager>().GetIfFlammable(other.GetComponentInParent<Renderer>().sharedMaterial))
                {

                    if (!other.GetComponentInChildren<FireWithTemperatureAndFuel>())
                    {
                        FindObjectOfType<FireSystemManager>().AddFireSystem(other.gameObject);
                    }
                    SpreadTemperature(other.GetComponentInChildren<FireWithTemperatureAndFuel>());
                }
            }
        }
    }

    private void SpendFuel()
    {
        float fuelBurned = Mathf.Lerp(minimumFuelBurn, fuelBurnRate, currentFuel/starterFuel);
        if (currentFuel < fuelBurned)
        {
            fuelBurned = currentFuel;
        }
        currentFuel -= fuelBurned;
        currentTemperature += fuelBurned * temperatureGainedPerFuelUnit;
    }

    private void RadiateHeat()
    {
        currentTemperature -= temperatureLossRate;
    }

    public void StartFire()
    {
        isAlight = true;
        fireLight.enabled = true;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Play();
        }
        SpendFuel();
    }

    private void Extinguish()
    {
        isAlight = false;
        fireLight.enabled = false;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Stop();
        }
    }

    private void AdjustParticleSize()
    {
        for(int i=0;i<fireParticleSystems.Length;i++)
        {
            var main = fireParticleSystems[i].main;
            main.startSize = particleStartSize[i] * Mathf.Lerp(0, maxFireSizeMultiplier, 1 - Mathf.Pow((currentFuel / starterFuel - 0.5f) * 2, 2f));
        }
    }

    public bool GetIsAlight()
    {
        return isAlight;
    }

    public void SpreadTemperature(FireWithTemperatureAndFuel other)
    {
        other.AddTemperature(1f);
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
