using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveTowardsCenter : MonoBehaviour
{
    bool isPulled = false;
    GameObject dest;
    // Movement speed in units per second.
    public float speed = 200f;

    void Start()
    {
        dest = GameObject.Find("BlackHole");
    }

    // Update is called once per frame
    void Update()
    {

         transform.position = Vector3.MoveTowards(transform.position, dest.transform.position, Time.deltaTime * speed);
    }

}
