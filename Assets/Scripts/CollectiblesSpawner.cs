using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PathVariables))]
public class CollectiblesSpawner : MonoBehaviour
{
    [Header("Spawning Controls")]
    [Tooltip("Start and end positions for spawn position calculation. Control is used if the spawn needs to happen in a curve")]
    public Transform start, end, control;
    [Tooltip("Number of collectables that will spawn. They will be equaly spaced")]
    public int numOfCollectibles = 10;
    [Tooltip("Prefab for collectable")]
    public GameObject collectiblePrefab;

    [Header("Enable Controls")]
    [Tooltip("True = spawn in curve, False = spwan in straight line")]
    public bool isCurve = false;
    [Tooltip("True = spawns collectables, False = does nothing")]
    public bool isSpawning = true;


    private PathVariables pathVariable;
    private Vector3 startPos, endPos, controlPos;
    private GameObject collectible;

    // Use this for initialization
    void Start()
    {
        pathVariable = GetComponent<PathVariables>();

        if (start == null)
        {
            start = transform.Find("Entrance");
            startPos = transform.TransformPoint(LocationDecode.locationToLocalPos[pathVariable.entranceLocation] + start.localPosition); ;
        }
        else
        {
            startPos = start.position;
        }

        if (end == null)
        {
            end = transform.Find("Exit");
            endPos = transform.TransformPoint(end.transform.localRotation * LocationDecode.locationToLocalPos[pathVariable.exitLocation] + end.localPosition);
        }
        else
        {
            endPos = end.position;
        }

        if (control == null)
        {
            //center of path
            control = transform;
        }
        else
        {
            controlPos = control.position;
        }


        if (isSpawning)
        {
            collectible = new GameObject();
            collectible.transform.parent = transform;
            collectible.name = "Collectables";
            if (isCurve)
            {
                DrawQuadraticCurve();
            }
            else
            {
                DrawLinearCurve();
            }
        }
    }


    private void DrawQuadraticCurve()
    {
        for (int i = 1; i < numOfCollectibles + 1; i+=2)
        {
            float t = i / (float)numOfCollectibles;
            GameObject collectable = (GameObject)Instantiate(collectiblePrefab, CalculateQuadraticBezierPoint(t, startPos, endPos, controlPos), end.rotation);
            collectable.transform.parent = this.collectible.transform;
            collectable.GetComponent<Collectibles>().pitch = 1 + ((i - 1) / 10);
        }
    }

    private void DrawLinearCurve()
    {
        for (int i = 1; i < numOfCollectibles + 1; i+=2)
        {
            float t = i / (float)numOfCollectibles;
            GameObject collectable = (GameObject)Instantiate(collectiblePrefab, CalculateLinearBezirPoint(t, startPos, endPos), end.rotation);
            collectable.transform.parent = this.collectible.transform;
            collectable.GetComponent<Collectibles>().pitch = 1 + ((i - 1) / 10);
        }
    }

    private Vector3 CalculateLinearBezirPoint(float t, Vector3 start, Vector3 end)
    {
        return start + t * (end - start);
    }


    private Vector3 CalculateQuadraticBezierPoint(float t, Vector3 start, Vector3 end, Vector3 control)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;

        Vector3 p = uu * start;
        p += 2 * u * t * control;
        p += tt * end;

        return p;
    }
}
