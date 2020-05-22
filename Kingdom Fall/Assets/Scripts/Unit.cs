using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float health;
    public float speed;

    public Transform target;
    
    Vector3[] path;
    int targetIndex;

    public void MoveToTarget(Transform newTarget)
    {
        target = newTarget;

        if (target != null)
        {
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
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
                    yield break;
                }
                currenttWayPoint = path[targetIndex];
            }
            transform.position = Vector3.MoveTowards(transform.position, currenttWayPoint, speed * Time.deltaTime);
            yield return null;
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
