using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;

public class AnimationCycling : MonoBehaviour
{

    Rigidbody rb;
    Animator anim;
    public float forwardSpeed = 7.0f;
    public float backwardSpeed = 2.0f;
    public float rotateSpeed = 2.0f;
    private Vector3 velocity;

    public bool isMoving = true;
    GameObject blackHole;

    PostProcessVolume post;
    Vignette vig;
    IKControl ik;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        ik = GetComponent<IKControl>();
        blackHole = GameObject.Find("BlackHole");
        post = Camera.main.GetComponent<PostProcessVolume>();
        post.profile.TryGetSettings(out vig);
    }

    private void Update()
    {
        if (transform.position.y < -60)
        {
            SceneManager.LoadScene("GameOver");
        }
    }
    private void FixedUpdate()
    {
        if (isMoving)
        {
            float v = Input.GetAxis("Vertical");
            float h = Input.GetAxis("Horizontal");
            anim.SetFloat("Speed", v);
            anim.SetFloat("Direction", h);

            //velocity = new Vector3(0, 0, v);
            //velocity = transform.TransformDirection(velocity);

            Vector3 movement = new Vector3(h * rotateSpeed, rb.velocity.y + (Physics.gravity.y * Time.deltaTime * 10f), v * forwardSpeed);
            if (v < 0)
            {
                movement.z = v * backwardSpeed;
            }
            //movement += Physics.gravity * Time.deltaTime * 100f;
            movement = transform.TransformDirection(movement);
            rb.velocity = movement;

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
            }
            else
            {
                if (anim.GetBool("isMoving") == true)
                {
                    anim.SetBool("isMoving", false);
                    ik.ikActive = false;
                }
            }

            //transform.localPosition += velocity * Time.fixedDeltaTime;

            transform.Rotate(0, h * rotateSpeed, 0);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                anim.SetBool("Jump", true);
                ik.ikActive = false;
            }
        } else
        {
            anim.SetBool("Death", true);
            vig.intensity.value += .01f;
            vig.smoothness.value += .01f;
            transform.position = Vector3.MoveTowards(transform.position, blackHole.transform.position, Time.deltaTime * forwardSpeed);
            if (Vector3.Distance(transform.position, blackHole.transform.position) < 2f)
            {
                StartCoroutine(LoadSceneAfterDelay(1f));
            }
        }
    }

    IEnumerator LoadSceneAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("GameOver");
    }

}



