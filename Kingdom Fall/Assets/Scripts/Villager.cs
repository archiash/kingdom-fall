﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Villager : Unit
{

    void Update()
    {
        if (enermyTarget != null)
        {
            if (Vector3.Distance(transform.position, enermyTarget.transform.position) < sightRange)
            {
                if (Vector3.Distance(transform.position, enermyTarget.transform.position) < attackRange)
                {
                    StopCoroutine("FollowPath");
                    Debug.Log("Attack");
                }
                else if (Vector3.Distance(transform.position, enermyTarget.transform.position) > attackRange)
                {
                    if (!onFollowPath)
                    {
                        MoveToTarget(enermyTarget.transform);
                        onFollowPath = true;
                    }

                }

            }
            else if (Vector3.Distance(transform.position, enermyTarget.transform.position) > sightRange)
            {
                enermyTarget = null;
            }
        }

        if (enermyTarget == null)
        {
            if (!onFollowPath) ;
                

            UpdateTarget();
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Villager))]
public class VillagerEditor : Editor
{

    private void OnSceneGUI()
    {
        Villager u = (Villager)target;
        Handles.color = Color.green;
        Handles.DrawWireArc(u.transform.position, Vector3.forward, Vector3.up, 360, u.sightRange);
        Handles.color = Color.red;
        Handles.DrawWireArc(u.transform.position, Vector3.forward, Vector3.up, 360, u.attackRange);
    }
}

#endif