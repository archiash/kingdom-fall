using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class CrystalTower : Building
{
    Transform target = null;

    public Arrow arrow;

    public float range;
    public float damage;
    public float firePerSec;

    float nextFireTime = 0f;

    public Vector2 firePointOffset;
    Vector3 firePoint;

    private void Awake()
    {
        firePoint = transform.position + new Vector3(firePointOffset.x, firePointOffset.y, 0);
    }

    void Update()
    {

        if (nextFireTime < 1 / firePerSec)
        {
            nextFireTime += Time.deltaTime;
        }

        if (target != null)
        {
            if (Vector3.Distance(transform.position, target.position) < range)
            {
                if (nextFireTime >= firePerSec)
                {
                    nextFireTime = 0f;
                    Arrow newArrow = Instantiate(arrow, firePoint, Quaternion.identity);
                    newArrow.SetTarget(firePoint, target, damage);
                }

            }
            else if (Vector3.Distance(transform.position, target.position) > range)
            {
                target = null;
            }
        }

        if (target == null)
        {
            UpdateTarget();
        }
    }

    void UpdateTarget()
    {
        GameObject[] enermies = GameObject.FindGameObjectsWithTag("Enermy");
        float dstToNearet = Mathf.Infinity;
        GameObject nearestEnermy = null;

        foreach (var enermy in enermies)
        {
            float dstToEnermy = Vector3.Distance(transform.position, enermy.transform.position);
            if (dstToEnermy < dstToNearet)
            {
                dstToNearet = dstToEnermy;
                nearestEnermy = enermy;
            }
        }

        if (nearestEnermy != null && dstToNearet < range)
        {
            target = nearestEnermy.transform;
        }
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(CrystalTower))]
public class CrystalTowerEditor : Editor
{
    private void OnSceneGUI()
    {
        CrystalTower t = (CrystalTower)target;
        Handles.color = Color.black;
        Handles.DrawWireArc(t.transform.position, Vector3.forward, Vector3.up, 360, t.range);
        Handles.DrawWireCube(t.transform.position + new Vector3(t.firePointOffset.x, t.firePointOffset.y, 0), Vector3.one * 0.05f);
    }
}
#endif
