using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCycling : MonoBehaviour
{

    Rigidbody rb;
    Animator anim;
    public float forwardSpeed = 7.0f;
    public float backwardSpeed = 2.0f;
    public float rotateSpeed = 2.0f;
    private Vector3 velocity;

    IKControl ik;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        ik = GetComponent<IKControl>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical");                
        float h = Input.GetAxis("Horizontal");
        anim.SetFloat("Speed", v);
        anim.SetFloat("Direction", h);

        velocity = new Vector3(0, 0, v);       
        velocity = transform.TransformDirection(velocity);

        if (v > 0.1)
        {
            velocity *= forwardSpeed;
            if (anim.GetBool("isMoving") == false)
            {
                anim.SetBool("isMoving", true);
                ik.ikActive = true;
            }
        }
        else if (v < -0.1)
        {
            velocity *= backwardSpeed;
            if (anim.GetBool("isMoving") == false)
            {
                anim.SetBool("isMoving", true);
                ik.ikActive = true;
            }
        } else
        {
            if (anim.GetBool("isMoving") == true)
            {
                anim.SetBool("isMoving", false);
                ik.ikActive = false;
            }
        }

        transform.localPosition += velocity * Time.fixedDeltaTime;

        transform.Rotate(0, h * rotateSpeed, 0);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetBool("Jump", true);
            ik.ikActive = false;
        }

    }

}



