using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireIgniter : MonoBehaviour
{
    public float ignitionRadius;
    private FireSystemManager fireManager;

    // Start is called before the first frame update
    void Start()
    {
        fireManager = FindObjectOfType<FireSystemManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!collision.collider.GetComponentInChildren<FireSystem>())
        {
            fireManager.AddFireSystem(collision.collider.gameObject);
        }
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, ignitionRadius);
        foreach(Collider hit in hitColliders)
        {
            if(hit.GetComponent<FireSystem>())
            {
                hit.GetComponent<FireSystem>().Ignite();
            }
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.GetComponent<FireSystem>())
        {
            collider.GetComponent<FireSystem>().Ignite();
        }
    }
}
