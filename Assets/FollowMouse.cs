using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    [SerializeField]
    Transform reference;

    public float x;
    // Update is called once per frame
    void Update()
    {
        Vector3 ikGoal = Camera.main.ScreenToWorldPoint(Input.mousePosition + Vector3.forward *3);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Input.GetButtonDown("Fire1"))
        {
            Debug.DrawRay(Camera.main.transform.position, ray.direction, Color.black);
            transform.position = ray.GetPoint(x);
        } else
        {
            print(ikGoal);
            transform.position = ikGoal;
        }

    }
}
