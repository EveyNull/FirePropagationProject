using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireManager : MonoBehaviour
{
    public float timeToStartFire = 5f;

    private bool isAlight = false;
    private float fireStartTimer = 0f;

    private ParticleSystem[] fireParticleSystems;
    private Light fireLight;

    // Start is called before the first frame update
    void Start()
    {
        fireParticleSystems = GetComponentsInChildren<ParticleSystem>();
        fireLight = GetComponentInChildren<Light>();
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.GetComponent<FireManager>())
        {
            if(other.GetComponent<FireManager>().GetIsAlight())
            {
                fireStartTimer += Time.deltaTime;
                if(fireStartTimer >= timeToStartFire)
                {
                    StartFire();
                }
            }
        }
    }

    public void StartFire()
    {
        isAlight = true;
        fireLight.enabled = true;
        foreach(ParticleSystem ps in fireParticleSystems)
        {
            ps.Play();
        }
    }

    public bool GetIsAlight()
    {
        return isAlight;
    }
}
