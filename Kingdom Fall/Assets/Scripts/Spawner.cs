using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform target;
    public Unit enermy;

    public float spawnRate;
    float nextSpawn = 0f;

    public void Update()
    {
        if(nextSpawn < 1)
        {
            nextSpawn += Time.deltaTime * spawnRate;
        }else if(nextSpawn >= 1)
        {
            nextSpawn = 0f;
            SpawnEnermy();
        }
    }

    public void SpawnEnermy()
    {
        Unit newEnermy = Instantiate(enermy,transform.position,Quaternion.identity);
        newEnermy.MoveToTarget(target);
    }
}
