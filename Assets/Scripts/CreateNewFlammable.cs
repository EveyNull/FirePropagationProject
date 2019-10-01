using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateNewFlammable : MonoBehaviour
{

    public GameObject flammablePrefab;
    public Transform flammableParent;

    private Camera cam;
    private void Start()
    {
        cam = GetComponent<Camera>();
    }
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit = new RaycastHit();

            if(Physics.Raycast(ray, out hit, 300f, -1, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider.tag == "Floor")
                {
                    Instantiate(flammablePrefab, new Vector3(hit.point.x, 1f, hit.point.z), new Quaternion(), flammableParent);
                }
            }
        }
    }
}
