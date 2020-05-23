using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public Transform target;
    public Unit enermy;

    public float spawnSpeed;
    float nextSpawn = 0f;

    public void Update()
    {
        if(nextSpawn < 1)
        {
            nextSpawn += Time.deltaTime * spawnSpeed;
        }else if(nextSpawn >= 1)
        {
            nextSpawn = 0f;
            SpawnEnermy();
        }
    }

    public void SpawnEnermy()
    {
        Unit newEnermy = Instantiate(enermy,transform.position,Quaternion.identity);
        newEnermy.enermyBase = target;
    }
}
