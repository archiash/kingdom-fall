using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public Transform target;

    Vector3 startPos;
    Vector3 lastTargetPost;

    Vector3 point;
    public float height = 5f;

    public float count = 0f;

    public float damage;
    public float speed;

    List<Transform> inDamageRange = new List<Transform>(); 

    // Update is called once per frame
    void Update()
    {    
        if(target != null)
        {
            if(lastTargetPost != target.position)
                lastTargetPost = target.position;
        }

        if(count <= 1f)
        {
            count += Time.deltaTime * speed;
        }

        Vector3 m1 = Vector3.Lerp(startPos, point, count);
        Vector3 m2 = Vector3.Lerp(point,lastTargetPost ,count);
        if(count < 1)
        {
            transform.rotation = LookAtTarget(Vector3.Lerp(m1, m2, count) - transform.position);
            transform.position = Vector3.Lerp(m1, m2, count);
        }
        
        if(count >= 1f)
        {            
            if(inDamageRange.Contains(target))
            {
                Unit enermy = target.GetComponent<Unit>();
                enermy.health -= damage;
                Destroy(this.gameObject);
            }
            Destroy(this.gameObject, 2f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        inDamageRange.Add(collision.transform);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        inDamageRange.Remove(collision.transform);
    }

    public static Quaternion LookAtTarget(Vector3 target)
    {
        float angle = Mathf.Atan2(target.y, target.x) * Mathf.Rad2Deg;
        return Quaternion.Euler(0, 0,angle  -90);
    }

    public void SetTarget(Transform start,Transform target,float damage)
    {
        this.damage = damage;
        startPos = start.position;
        this.target = target;
        point = ((startPos + target.position) / 2) + Vector3.up * height;
    }

    public void SetTarget(Vector3 start, Transform target, float damage)
    {
        this.damage = damage;
        startPos = start;
        this.target = target;
        point = ((startPos + target.position) / 2) + Vector3.up * height;
    }
}
