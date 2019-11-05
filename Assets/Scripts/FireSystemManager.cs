using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystemManager : MonoBehaviour
{
    public List<Material> flammables;
    public GameObject fireSystemPrefab;

    public bool GetIfFlammable(Material mat)
    {
        bool test = flammables.Contains(mat);
        return flammables.Contains(mat);
    }

    public void AddFireSystem(GameObject other)
    {
        GameObject newFireSystem = Instantiate(fireSystemPrefab);
        newFireSystem.transform.SetParent(other.transform);
        newFireSystem.transform.localPosition = new Vector3(0f, 0f, 0f);
        
    }
}
