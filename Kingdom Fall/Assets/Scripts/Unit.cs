using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float health;
    public float speed;
    public float sightRange;
    public float attackRange;

    public Transform enermyBase;
    public string enermyTag;
    public Unit enermyTarget;

    public bool onFollowPath = false;

    [HideInInspector]
    public List<Unit> enermyInMap = new List<Unit>();

    Vector3[] path;
    int targetIndex;

    public void MoveToTarget(Transform newTarget)
    {
        enermyBase = newTarget;

        if (enermyBase != null)
        {
            onFollowPath = true;
            PathRequestManager.RequestPath(transform.position, enermyBase.position, OnPathFound);
        }
    }

    private void Update()
    {
        if(health <= 0)
        {
            Destroy(this.gameObject);
        }
    }

    public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = newPath;
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }            
    }

    IEnumerator FollowPath()
    {
        Vector3 currenttWayPoint = path[0];

        while(true)
        {
            if(transform.position == currenttWayPoint)
            {
                targetIndex++;
                if(targetIndex >= path.Length)
                {
                    onFollowPath = false;
                    yield break;
                }
                currenttWayPoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currenttWayPoint, speed * Time.deltaTime);
            yield return null;
        }
    }

    public void UpdateTarget()
    {
        GameObject[] enermies = GameObject.FindGameObjectsWithTag(enermyTag);
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

        if (nearestEnermy != null && dstToNearet < sightRange)
        {
            onFollowPath = false;
            enermyTarget = nearestEnermy.GetComponent<Unit>();
            if(enermyTarget.enermyTarget == null)
            {
                enermyTarget.enermyTarget = this;
            }
        }
    }
    public void OnDrawGizmos()
    {
        if(path != null)
        {
            for(int i = targetIndex; i < path.Length;i++)
            {
                Gizmos.color = Color.red;
                //Gizmos.DrawWireCube(path[i], new Vector3(1, 1, 0));
                if(i==targetIndex)
                {
                    Gizmos.DrawLine(transform.position, path[i]);
                }else
                {
                    Gizmos.DrawLine(path[i-1], path[i]);
                }
            }
        }
    }
} 

