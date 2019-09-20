using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class PathVariables : MonoBehaviour {
    [Header("Path")]
    [Tooltip("Entrance Location")]
    public int entranceLocation;
    [Tooltip("Exit Location")]
    public int exitLocation;
    [Tooltip("Distance of center pivot to entrance or exit")]
    public float sizeOfPath;
    public bool isTurn = false;

    public Material corruption;
    public Renderer mesh;

    public bool destroy = false;

    float score = 10f;

    GameObject entrance;
    GameObject exit;

    bool spread = false;

    BoxCollider entranceCollider;

    public LocationDecode.Direction dir;
    public LocationDecode.PathType type;

    public Explode ex;

    private void Awake()
    {
        if (!isTurn)
        {
            entrance = new GameObject();
            entrance.name = "Entrance";
            entrance.transform.parent = gameObject.transform;
            entrance.transform.localPosition = new Vector3(0, 0, -sizeOfPath);
            entrance.transform.localRotation = Quaternion.identity;
            entrance.tag = "Path";
            entranceCollider = entrance.AddComponent<BoxCollider>();
            entranceCollider.size = new Vector3(360, 180, 0);
            entranceCollider.isTrigger = true;
            exit = new GameObject();
            exit.name = "Exit";
            exit.transform.parent = gameObject.transform;
            exit.transform.localPosition = new Vector3(0, 0, sizeOfPath);
            exit.transform.localRotation = Quaternion.identity;
        } else
        {
            entrance = transform.Find("Entrance").gameObject;
            entranceCollider = entrance.GetComponent<BoxCollider>();
            exit = transform.Find("Exit").gameObject;
        }

        //copy and make a new instance of material
        Material mat = new Material(corruption);
        mesh.material = mat;
        mat.SetTextureScale("_MainTex", new Vector2(Random.Range(.7f, 1.5f), Random.Range(.7f, 1.5f)));
        
    }

    private void OnDrawGizmos()
    {

        Gizmos.color = Color.magenta;
        if (entrance != null && exit != null)
        {
            Vector3 entrancePos = transform.TransformPoint(LocationDecode.locationToLocalPos[entranceLocation] + entrance.transform.localPosition);
            Vector3 exitPos = transform.TransformPoint(exit.transform.localRotation * LocationDecode.locationToLocalPos[exitLocation] + exit.transform.localPosition);
            if (isTurn)
            {
                for (int i = 1; i < 50 + 1; i++)
                {
                    float t = i / 50f;
                    Gizmos.DrawCube(CalculateQuadraticBezierPoint(t, entrancePos,  transform.position, exitPos), new Vector3(1,1,1));
                }
            }
            else
            {
                Gizmos.DrawLine(entrancePos, exitPos);
            }
        }
    }

    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * p0;
        p += 2 * u * t * p1;
        p += tt * p2;

        return p;

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerBody"))
        {
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("BlackHole"))
        {
            spread = true;
            //StartCoroutine(ChangeTriggerAfterDelay());
            //queue corrption
        }
    }

    IEnumerator ChangeTriggerAfterDelay()
    {
        yield return new WaitForSeconds(.5f);
        //entranceCollider.isTrigger = false;
        entrance.layer = 17;
    }

    float current = 0;
    float end = 1.5f;
    private void Update()
    {
        if (mesh != null)
        {
            if (spread)
            {
                if (current >= end)
                {
                    spread = false;
                    if (destroy)
                    {
                        StartCoroutine(ex.SplitMesh(false));
                        Destroy(gameObject);
                    }
                }
                else
                {
                    if (isTurn)
                    {
                        end = 2f;
                    }
                    current += Time.deltaTime;
                    float percent = current / end;
                    Mathf.Clamp01(percent);
                    mesh.material.SetFloat("_Cutoff", percent);
                }
            }
        }
    }

}
