using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsCenter : MonoBehaviour
{
    bool isPulled = false;
    Transform dest;
    // Movement speed in units per second.
    public float speed = 1.0F;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (isPulled)
        {
            transform.position = Vector3.Lerp(transform.position, dest.position, Time.deltaTime*speed);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isPulled)
        {
            isPulled = true;
            dest = other.transform;
        } else
        {
            isPulled = false;
            Destroy(gameObject);
        }
    }
}
