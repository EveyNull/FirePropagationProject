using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystemManager : MonoBehaviour
{
    [System.Serializable]
    public class FlammableMaterial
    {
        public Material material;
        public float igniteTemperature;
        public float lifeSpanSeconds;
        public float maxFireSizeMultiplier;
        public float spreadTemperature;
    }

    
    public List<FlammableMaterial> flammableMats;

    public GameObject fireSystemPrefab;

    public float meshFireDistanceBetween = 5f;

    private List<FireSystem> fireSystems;

    public Vector3 windDirection;

    private float timer = 0f;

    private void Start()
    {
        fireSystems = new List<FireSystem>();
        foreach (FireSystem fs in FindObjectsOfType<FireSystem>())
        {
            fireSystems.Add(fs);
        }

        windDirection = new Vector3();
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if(timer >= 60f)
        {
            timer = 0f;
            windDirection = new Vector3(Random.Range(-2, 2), 0f, Random.Range(-2,2));
            ChangeWindDirection(windDirection);
        }
    }

    void ChangeWindDirection(Vector3 newWind)
    {
        foreach(FireSystem fs in fireSystems)
        {
            fs.AdjustParticleDirection(newWind);
        }
    }

    public bool GetIfFlammable(Material mat)
    {
        foreach(FlammableMaterial flammable in flammableMats)
        {
            if(flammable.material == mat)
            {
                return true;
            }
        }
        return false;
    }

    public void RemoveFireSystem(FireSystem fs)
    {
        fireSystems.Remove(fs);
    }

    public FlammableMaterial GetMaterialProperties(Material mat)
    {
        foreach (FlammableMaterial flammable in flammableMats)
        {
            if (flammable.material == mat)
            {
                return flammable;
            }
        }
        return null;
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
            SetupFireSystem(newFireSystem.GetComponent<FireSystem>(), newFireSystem.GetComponentInParent<Renderer>().sharedMaterial);
            fireSystems.Add(newFireSystem.GetComponent<FireSystem>());
        }
    }

    private void SetupFireSystem(FireSystem fs, Material mat)
    {
        FlammableMaterial fm = GetMaterialProperties(mat);
        if (fm != null)
        {
            fs.igniteTemperature = fm.igniteTemperature;
            fs.lifeSpanSeconds = fm.lifeSpanSeconds;
            fs.maxFireSizeMultiplier = fm.maxFireSizeMultiplier;
            fs.spreadTemperature = fm.spreadTemperature;
            fs.particleDirection = windDirection;
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
            float xTransform = Mathf.Clamp(
                mesh.bounds.min.x + (i % numberFiresRow * meshFireDistanceBetween) + Random.Range(-1, 1f)
                , mesh.bounds.min.x
                , mesh.bounds.max.x);
            float zTransform = Mathf.Clamp(
                mesh.bounds.min.z + Mathf.FloorToInt(i / numberFiresRow) * meshFireDistanceBetween + Random.Range(-1, 1f)
                , mesh.bounds.min.z
                , mesh.bounds.max.z);

            GameObject newFireSystem = Instantiate(fireSystemPrefab, new Vector3(xTransform, 0f, zTransform), fireSystemPrefab.transform.rotation, mesh.transform);
            newFireSystem.GetComponent<SphereCollider>().radius /= mesh.transform.localScale.x * 2;
            SetupFireSystem(newFireSystem.GetComponent<FireSystem>(), mesh.GetComponentInParent<Renderer>().sharedMaterial);
            fireSystems.Add(newFireSystem.GetComponent<FireSystem>());
        }
    }
}
