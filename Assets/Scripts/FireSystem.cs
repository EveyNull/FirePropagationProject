using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class FireSystem : MonoBehaviour
{

    public float igniteTime = 5f;
    public float timeIgnited;
    private bool beingIgnited;

    public float lifeSpanSeconds = 10f;
    public float lifeRemaining;

    public float maxFireSizeMultiplier = 2;

    public bool isAlight = false;

    private FireSystemManager fireManager;

    private Material thisMaterial;
    private ParticleSystem[] fireParticleSystems;
    private float[] particleStartSize;
    private Light fireLight;

    private Renderer ashesRenderer;

    public Vector3 windDirection;

    public bool breaksOnExtinguish;

    private bool igniteAtFrameEnd = false;

    private float startFireRadius;

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
        timeIgnited = 0f;
        lifeRemaining = lifeSpanSeconds;
        startFireRadius = GetComponent<SphereCollider>().radius;
        try
        {
            GameObject ashes = fireManager.GetMaterialProperties(GetComponentInParent<Renderer>().sharedMaterial).ashesPrefab;
            if (ashes != null)
            {
                ashesRenderer = Instantiate(ashes, transform.position, transform.rotation, transform).GetComponentInChildren<Renderer>();
                ashesRenderer.enabled = false;
            }
        }
        catch(NullReferenceException e)
        {
            Debug.LogException(e);
        }
        
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
            if(beingIgnited)
            {
                if ((timeIgnited += Time.deltaTime) >= igniteTime && lifeRemaining > 0)
                {
                    StartFire();
                }
            }
        }
    }

    private void LateUpdate()
    {
        beingIgnited = false;
        if(igniteAtFrameEnd)
        {
            StartFire();
            igniteAtFrameEnd = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAlight)
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, other.transform.position - transform.position, out hit);
            if(hit.collider != other)
            {
                return;
            }
            if (other.GetComponentInParent<Renderer>())
            {
                if (fireManager.GetIfFlammable(other.GetComponentInParent<Renderer>().sharedMaterial))
                {
                    if (!other.GetComponentInChildren<FireSystem>())
                    {
                        fireManager.AddFireSystem(other.gameObject);
                    }
                    else if (!other.GetComponent<MeshCollider>())
                    {
                        BurnOther(other.GetComponentInChildren<FireSystem>());
                    }
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
        gameObject.layer = fireManager.fireAlightLayer;
        foreach (ParticleSystem ps in fireParticleSystems)
        {
            ps.Play();
        }
        CountDownLife();
        GetComponent<SphereCollider>().radius = startFireRadius * 3f;
        AdjustParticleDirection(windDirection);
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
        if (ashesRenderer != null)
        {
            ashesRenderer.enabled = true;
        }
        if(breaksOnExtinguish)
        {
            Destroy(transform.parent.gameObject);
        }
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
        windDirection = newVel;
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
        igniteAtFrameEnd = true;
    }

    public bool GetIsAlight()
    {
        return isAlight;
    }

    public void BurnOther(FireSystem other)
    {
        other.SetBeingIgnited();
    }

    public void SetBeingIgnited()
    {
        beingIgnited = true;
    }
}
