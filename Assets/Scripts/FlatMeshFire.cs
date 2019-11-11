using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlatMeshFire : FireWithTemperatureAndFuel
{
    public GameObject[] neighbours = new GameObject[4];

    public float spreadFireTime = 5f;
    private float spreadFireTimer = 0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        if (isAlight)
        {
            spreadFireTimer += Time.deltaTime;
            if (spreadFireTimer >= spreadFireTime)
            {
                SpreadFireToNeighbour();
                spreadFireTimer = 0f;
            }
        }
        base.Update();
    }

    public void SpreadFireToNeighbour()
    {
        int spreadTo = Random.Range(0, 4);
        for (int i = 0; i < 4; i++)
        {
            if (neighbours[spreadTo] == null)
            {
                StartNewFire(spreadTo);
                break;
            }
            spreadTo = (spreadTo + 1) % 4;
        }
    }

    void StartNewFire(int direction)
    {
        Vector3 newPos = transform.position;
        switch (direction)
        {
            case 0:
                {
                    newPos.x += GetComponent<SphereCollider>().radius;
                    break;
                }
            case 1:
                {
                    newPos.z += GetComponent<SphereCollider>().radius;
                    break;
                }
            case 2:
                {
                    newPos.x -= GetComponent<SphereCollider>().radius;
                    break;
                }
            case 3:
                {
                    newPos.z -= GetComponent<SphereCollider>().radius;
                    break;
                }
        }
        neighbours[direction].AddComponent<FlatMeshFire>();
        neighbours[direction].transform.SetParent(transform.parent);
        neighbours[direction].transform.position = newPos;
        neighbours[direction].GetComponent<FlatMeshFire>().SetNeighbour(gameObject, (direction + 2) % 4);
    }

    void SetNeighbour(GameObject neighbour, int direction)
    {
        neighbours[direction] = neighbour;
    }
}
