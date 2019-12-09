using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSystemManager : MonoBehaviour
{
    [System.Serializable]
    public class FlammableMaterial
    {
        public bool fireSettingNoise;
        public Material material;
        public float igniteTime;
        public float lifeSpanSeconds;
        public float maxFireSizeMultiplier;
        public bool breaksOnExtinguish;
        public GameObject ashesPrefab;
    }

    public int fireAlightLayer;
    
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
        if (GetIfFlammable(other.GetComponentInParent<Renderer>().sharedMaterial))
        {
            if (other.GetComponent<MeshCollider>())
            {
                SetupFlammableTerrain(other.GetComponent<MeshCollider>());
            }
            else
            {
                GameObject newFireSystem = Instantiate(fireSystemPrefab);
                newFireSystem.transform.SetParent(other.transform, false);
                newFireSystem.transform.localPosition = new Vector3(0f, 0.5f, 0f);
                SetupFireSystem(newFireSystem.GetComponent<FireSystem>(), newFireSystem.GetComponentInParent<Renderer>().sharedMaterial);
                fireSystems.Add(newFireSystem.GetComponent<FireSystem>());
            }
        }
    }

    private void SetupFireSystem(FireSystem fs, Material mat)
    {
        FlammableMaterial fm = GetMaterialProperties(mat);
        if (fm != null)
        {
            fs.igniteTime = fm.fireSettingNoise ? fm.igniteTime + Random.Range(-1f, 1f) : fm.igniteTime;
            fs.lifeSpanSeconds = fm.fireSettingNoise ? fm.lifeSpanSeconds + Random.Range(-1f, 1f) : fm.lifeSpanSeconds;
            fs.maxFireSizeMultiplier = fm.fireSettingNoise ? fm.maxFireSizeMultiplier + Random.Range(-1f, 1) : fm.maxFireSizeMultiplier;
            fs.windDirection = windDirection;
            fs.breaksOnExtinguish = fm.breaksOnExtinguish;
        }
    }

    void SetupFlammableTerrain(MeshCollider mesh)
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
            Ray ray = new Ray(new Vector3(xTransform, mesh.bounds.max.y + 1f, zTransform), Vector3.down);
            RaycastHit[] hits = Physics.RaycastAll(ray);
            float yTransform = 0;
            foreach(RaycastHit hit in hits)
            {
                if(hit.collider == mesh)
                {
                    yTransform = hit.point.y;
                }
            }
            GameObject newFireSystem = Instantiate(fireSystemPrefab, new Vector3(xTransform, yTransform, zTransform), fireSystemPrefab.transform.rotation, mesh.transform);
            SetupFireSystem(newFireSystem.GetComponent<FireSystem>(), mesh.GetComponentInParent<Renderer>().sharedMaterial);
            Vector3 worldScale = newFireSystem.transform.lossyScale;
            newFireSystem.GetComponent<SphereCollider>().radius /= Mathf.Max(worldScale.x, Mathf.Max(worldScale.y, worldScale.z));
            fireSystems.Add(newFireSystem.GetComponent<FireSystem>());
        }
    }
}
