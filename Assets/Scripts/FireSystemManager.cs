using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystemManager : MonoBehaviour
{
    public List<Material> flammables;
    public GameObject fireSystemPrefab;

    public float meshFireDistanceBetween = 5f;

    public bool GetIfFlammable(Material mat)
    {
        bool test = flammables.Contains(mat);
        return flammables.Contains(mat);
    }

    public void AddFireSystem(GameObject other)
    {
        
        if(other.GetComponent<MeshCollider>())
        {
            SetupMeshFlammable(other.GetComponent<MeshCollider>());
        }
        else
        {
            GameObject newFireSystem = Instantiate(fireSystemPrefab);
            newFireSystem.transform.SetParent(other.transform);
            newFireSystem.transform.localPosition = new Vector3(0f, 0f, 0f);
            newFireSystem.AddComponent<FireWithTemperatureAndFuel>();
        }
    }

    void SetupMeshFlammable(MeshCollider mesh)
    {
        float xdistance = mesh.bounds.max.x - mesh.bounds.min.x;
        float zdistance = mesh.bounds.max.z - mesh.bounds.min.z;

        int numberFiresRow = Mathf.FloorToInt(xdistance / meshFireDistanceBetween) + 1;
        int numberColumns = Mathf.FloorToInt(zdistance / meshFireDistanceBetween) + 1;

        int numberOfFires = numberFiresRow * numberColumns;

        for(int i = 0; i < numberOfFires; i++)
        {
            float xTransform = mesh.bounds.min.x + (i % numberFiresRow * meshFireDistanceBetween) + Random.Range(-1, 1f);
            float zTransform = mesh.bounds.min.z + Mathf.FloorToInt(i / numberFiresRow) * meshFireDistanceBetween + Random.Range(-1, 1f);

            Instantiate(fireSystemPrefab, new Vector3(xTransform, 0f, zTransform), fireSystemPrefab.transform.rotation, mesh.transform).GetComponent<SphereCollider>().radius /= mesh.transform.localScale.x*2;
        }
    }
}
