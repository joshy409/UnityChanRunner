using UnityEngine;
using Unity.Collections;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/ProcedureVariables", order = 1)]
public class ProcedureVariables : ScriptableObject
{
    [Header("Block Prefabs")]
    [Tooltip("Straight Block Prefabs")]
    public GameObject[] straightPrefabs;
    [Tooltip("Turn Block Prefabs")]
    public GameObject[] turnPrefabs;
    //[Tooltip("Up Block Prefabs")]
    //public GameObject[] upPrefabs;
    //[Tooltip("Down Block Prefabs")]
    //public GameObject[] downPrefabs;
    //[Tooltip("Up Flipped Block Prefabs")]
    //public GameObject[] upFlipPrefabs;
    //[Tooltip("Down Flipped Block Prefabs")]
    //public GameObject[] downFlipPrefabs;
    //[Tooltip("Vertical Block Prefab")]
    //public GameObject[] verticalPrefabs;


    [Header("Block Constraints")]
    [Tooltip("Minimum number of straight cubes before next random cube")]
    public float minStraightCube = 3f;

    [HideInInspector]
    public int straightLength = 1;

    [Header("Spawning Variables")]
    [Tooltip("% Chance the next random cube will be a straight cube")]
    [Range(1, 50)] public int straightWeight = 33;

    [ReadOnly]
    [Tooltip("% Chance the next random cube will be a turn cube. ReadOnly")]
    [Range(50, 99)] public int turnWeight = 67;

    [Tooltip("Spawn Up and Down")]
    public bool upDownEnabled = false;
    [Tooltip("Spawn Up and Down Turns")]
    public bool upDownTurnEnabled = false;

    [Header("Deleting Variable")]
    [Tooltip("Minimum number of path required before deletion")]
    public int minPath = 2;
    [Tooltip("Destory delay in seconds")]
    public float seconds = 4f;
}