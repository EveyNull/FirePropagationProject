using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireIgniter : MonoBehaviour
{
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
    }

    private void OnTriggerEnter(Collider collider)
    {
        if(collider.GetComponent<FireSystem>())
        {
            collider.GetComponent<FireSystem>().Ignite();
        }
    }
}
