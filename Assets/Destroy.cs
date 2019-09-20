using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Path")
        {
            PathVariables pv = other.gameObject.GetComponent<PathVariables>();
            if (!pv.isTurn)
            {
                if (pv.mesh != null)
                {
                    pv.mesh.material.SetFloat("_Cutoff", 1f);
                    StartCoroutine(pv.ex.SplitMesh(false));
                }
                Destroy(other.gameObject);
            }

        }
        else
        {
            Destroy(other.gameObject);
        }
    }
}
