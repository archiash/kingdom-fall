using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Orc : Unit
{

    float nextAttack = 0f;
    void Update()
    {

        if (health <= 0)
        {
            Destroy(this.gameObject);
        }

        if (nextAttack < 1)
        {
            nextAttack += Time.deltaTime * attackSpeed;
        }

        if (enermyTarget != null)
        {
            if (Vector3.Distance(transform.position, enermyTarget.transform.position) < sightRange)
            {
                if (Vector3.Distance(transform.position, enermyTarget.transform.position) < attackRange)
                {
                    StopCoroutine("FollowPath");
                    onFollowPath = false;
                    if (nextAttack >= 1)
                    {                       
                        enermyTarget.health -= attackDamage;
                        nextAttack = 0f;
                    }

                }
                else if (Vector3.Distance(transform.position, enermyTarget.transform.position) > attackRange)
                {
                    if(!onFollowPath)
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
            if(!onFollowPath && !IsEnermyBase())
                MoveToTarget(enermyBase);

            if(IsEnermyBase())
            {
                Destroy(gameObject);
            }

            UpdateTarget();
        }
    }
}

#if UNITY_EDITOR

[CustomEditor(typeof(Orc))]
public class OrcEditor : Editor
{

    private void OnSceneGUI()
    {
        Orc u = (Orc)target;
        Handles.color = Color.green;
        Handles.DrawWireArc(u.transform.position, Vector3.forward, Vector3.up, 360, u.sightRange);
        Handles.color = Color.red;
        Handles.DrawWireArc(u.transform.position, Vector3.forward, Vector3.up, 360, u.attackRange);
    }
}

#endif