using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(SphereCollider))]
[RequireComponent(typeof(CapsuleCollider))]
public class Collectibles : MonoBehaviour {

    [SerializeField] Vector3 rotationValues;
    [SerializeField] float minMoveSpeed;
    [SerializeField][Range(0f, 1f)] float shrinkPercent;
    [SerializeField] float score = 5f;
    [SerializeField] GameObject obj;

    bool homing;
    float moveSpeed;
    Transform player;
    //PlayerController pc;
    AudioSource audioSource;

    public float pitch = 1f;

    private void Start() {
        audioSource = GetComponent<AudioSource>();
    }


    void Update() {
        obj.transform.Rotate(rotationValues);
        if (homing) {
            //moveSpeed = minMoveSpeed + (pc.CurrentForce.z - 100000) / 10000;
            transform.position += (player.position - transform.position) * moveSpeed * Time.deltaTime;
            obj.transform.localScale = transform.localScale * shrinkPercent;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBody") && homing) {
            audioSource.pitch = pitch;
            audioSource.Play();
            ScoreClass.AddPoints(score);
            GetComponent<SphereCollider>().enabled = false;
            GetComponent<CapsuleCollider>().enabled = false;
            obj.GetComponent<MeshRenderer>().enabled = false;
            Destroy(gameObject, 1f);
        }
        if (other.CompareTag("PlayerBody")) {
            player = other.gameObject.transform;
            //pc = other.GetComponentInParent<PlayerController>();
            homing = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("PlayerBody")) {
            homing = false;
        }
    }
}
