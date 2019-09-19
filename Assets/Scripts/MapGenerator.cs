using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public struct Pos
{
    public Vector3 pos;
    public Quaternion rot;
}

public class MapGenerator : MonoBehaviour
{

    [Tooltip("Scriptable Object")]
    [SerializeField] ProcedureVariables generationVariables;
    [SerializeField] GameObject meshRenderer;
    [SerializeField] GameObject meshCollider;
    //[SerializeField] GameObject correctionCollider;
    Queue<GameObject> generatedPaths = new Queue<GameObject>();
    BoxCollider col;
    Transform exitPosition;
    PathVariables pathVariables;

    public Queue<Pos> lastGeneratedTurn = new Queue<Pos>();

    GameObject orientation;

    private bool disableMesh = false;

    LocationDecode.PathType lastGeneratedPathType;

    void Start()
    {
        col = GetComponent<BoxCollider>();
        pathVariables = GetComponent<PathVariables>();
        exitPosition = transform.Find("Exit");
    }

    private void Update()
    {
        generationVariables.turnWeight = 100 - generationVariables.straightWeight;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            
            GenerateUntilTurn();
            GenerateUntilTurn();
        }
    }


    bool hasUpDownBlockBeenGenerated = false;
    void GenerateUntilTurn()
    {
        bool hasTurnBlockBeenGenerated = false;

        GameObject path = new GameObject();
        path.name = "Path";
        //path.transform.parent = gameObject.transform;

        //TODO: is while loop the best option? (unity crashes if straight weight becomes very high)
        //currently set to 50% max for straight path to be chosen
        while (!hasTurnBlockBeenGenerated)
        {
            //generates minimum number of straight cubes
            if (generationVariables.straightLength <= generationVariables.minStraightCube)
            {
                if (!hasUpDownBlockBeenGenerated)
                {
                    GenerateStraightCube(path);
                    generationVariables.straightLength++;
                }
                else
                {
                    //TODO: generate vertical blocks instead of straight blocks
                    //GenerateVerticalCube(path);
                    generationVariables.straightLength++;
                }
            }

            //randomly picks a cube to generate
            else
            {
                col.center = (exitPosition.position - exitPosition.forward / 2 * pathVariables.sizeOfPath);

                int rand = Random.Range(1, 100);
                //print(rand);
                if (rand <= generationVariables.straightWeight)
                {
                    if (!hasUpDownBlockBeenGenerated)
                    {
                        GenerateStraightCube(path);
                    }
                    else
                    {
                        //GenerateVerticalCube(path);
                    }
                }
                else
                {
                    hasTurnBlockBeenGenerated = true;

                    if (hasUpDownBlockBeenGenerated)
                    {
                        hasUpDownBlockBeenGenerated = !hasUpDownBlockBeenGenerated;
                        if (lastGeneratedPathType == LocationDecode.PathType.Up)
                        {
                            //GenerateDownFilpCube(path);
                        } else if (lastGeneratedPathType == LocationDecode.PathType.Down)
                        {
                            //GenerateUpFilpCube(path);
                        }
                    }
                    else
                    {
                        if (generationVariables.upDownEnabled)
                        {
                            rand = Random.Range(0, 2);
                        } else
                        {
                            rand = 0;
                        }
                        //uncommenting this line will make the map generator only generate up and down blocks
                        //rand = 1;
                        if (rand == 0)
                        {
                            GenerateTurnCube(path);
                        }
                        else if (rand == 1)
                        {
                            hasUpDownBlockBeenGenerated = !hasUpDownBlockBeenGenerated;
                            rand = Random.Range(0, 2);
                            if (rand == 0)
                            {
                                //GenerateUpCube(path);
                            } else
                            {
                                //GenerateDownCube(path);
                            }
                        }
                    }
                }

                if (!hasUpDownBlockBeenGenerated)
                {
                    GenerateStraightCube(path);
                    generationVariables.straightLength = 1;
                }
                else
                {
                    //GenerateVerticalCube(path);
                    generationVariables.straightLength = 1;
                }
            }
        }
        generatedPaths.Enqueue(path);
    }


    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerBody"))
        {
            //print("called");
            if (generatedPaths.Count > generationVariables.minPath)
            {
                Destroy(generatedPaths.Dequeue(), generationVariables.seconds);
                //Destroy(generatedPaths.Dequeue(), generationVariables.seconds);
            }

            if (!disableMesh)
            {
                disableMesh = true;
                StartCoroutine(DisableAfterDelay());
            }

        }
    }
    IEnumerator DisableAfterDelay()
    {
        yield return new WaitForSeconds(2f);
        meshRenderer.SetActive(false);
        meshCollider.SetActive(false);
        //correctionCollider.SetActive(false);
    }

    private int ChooseCloseExit(int exitLocation)
    {
        List<int> validEntrances = new List<int>();

        Vector3 locationPos = LocationDecode.locationToLocalPos[exitLocation];
        
        for (int i = -1; i < 2; i += 2)
        {
            for (int j = -1; j < 2; j += 2)
            {
                int index = LocationDecode.locationToLocalPos.IndexOf(locationPos + new Vector3(LocationDecode.OFFSET * i, LocationDecode.OFFSET * j, 0));
                if (index > -1)
                {
                    validEntrances.Add(index);
                }
            }
        }

        for (int i = -1; i < 2; i += 2)
        {
            for (int j = -1; j < 2; j += 2)
            {
                int index = LocationDecode.locationToLocalPos.IndexOf(locationPos + new Vector3(LocationDecode.OFFSET/2 * i - LocationDecode.OFFSET/2 * j, LocationDecode.OFFSET/2 * j + LocationDecode.OFFSET/2 * i, 0));
                if (index > -1)
                {
                    validEntrances.Add(index);
                }
            }
        }

        return validEntrances[Random.Range(0, validEntrances.Count)];
    }



    private void GenerateStraightCube(GameObject path)
    {
        int exitLocation = pathVariables.exitLocation;
        int rand = Random.Range(0, generationVariables.straightPrefabs.Length);
        PathVariables prefabPathVariables = generationVariables.straightPrefabs[rand].GetComponent<PathVariables>();
        GameObject generatedCube = (GameObject)Instantiate(generationVariables.straightPrefabs[rand], exitPosition.position + exitPosition.forward * prefabPathVariables.sizeOfPath, exitPosition.rotation);
        PathVariables currentPathVar = generatedCube.GetComponent<PathVariables>();
        currentPathVar.entranceLocation = pathVariables.exitLocation;
        currentPathVar.exitLocation = ChooseCloseExit(exitLocation); ;
        pathVariables = currentPathVar;
        generatedCube.transform.parent = path.transform;
        exitPosition = generatedCube.transform.Find("Exit");
        generatedCube.name = pathVariables.entranceLocation + "_" + pathVariables.exitLocation + "_" + generatedCube.name;
    }

    private void GenerateTurnCube(GameObject path)
    {
        int exitLocation = pathVariables.exitLocation;
        int rand = Random.Range(0, generationVariables.turnPrefabs.Length);
        PathVariables prefabPathVariables = generationVariables.turnPrefabs[rand].GetComponent<PathVariables>();
        GameObject generatedCube = (GameObject)Instantiate(generationVariables.turnPrefabs[rand], exitPosition.position + exitPosition.forward * (prefabPathVariables.sizeOfPath), exitPosition.rotation);
        pathVariables = generatedCube.GetComponent<PathVariables>();
        generatedCube.transform.parent = path.transform;
        pathVariables.entranceLocation = exitLocation;
        exitPosition = generatedCube.transform.Find("Exit");
        generatedCube.name = pathVariables.entranceLocation + "_" + pathVariables.exitLocation + "_" + generatedCube.name;
        Pos copy = new Pos();
        copy.pos = generatedCube.transform.position;
        copy.rot = exitPosition.transform.rotation;
        lastGeneratedTurn.Enqueue(copy);
    }



    //private void GenerateUpCube(GameObject path)
    //{
    //    int exitLocation = pathVariables.exitLocation;
    //    int rand = Random.Range(0, generationVariables.upPrefabs.Length);
    //    PathVariables prefabPathVariables = generationVariables.upPrefabs[rand].GetComponent<PathVariables>();
    //    GameObject generatedCube = (GameObject)Instantiate(generationVariables.upPrefabs[rand], exitPosition.position + exitPosition.forward * (prefabPathVariables.sizeOfPath), exitPosition.rotation);
    //    pathVariables = generatedCube.GetComponent<PathVariables>();
    //    generatedCube.transform.parent = path.transform;
    //    pathVariables.entranceLocation = exitLocation;
    //    exitPosition = generatedCube.transform.Find("Exit");
    //    generatedCube.name = pathVariables.entranceLocation + "_" + pathVariables.exitLocation + "_" + generatedCube.name;
    //    lastGeneratedPathType = LocationDecode.PathType.Up;
    //}

    //private void GenerateDownCube(GameObject path)
    //{
    //    int exitLocation = pathVariables.exitLocation;
    //    int rand = Random.Range(0, generationVariables.downPrefabs.Length);
    //    PathVariables prefabPathVariables = generationVariables.downPrefabs[rand].GetComponent<PathVariables>();
    //    GameObject generatedCube = (GameObject)Instantiate(generationVariables.downPrefabs[rand], exitPosition.position + exitPosition.forward * (prefabPathVariables.sizeOfPath), exitPosition.rotation);
    //    pathVariables = generatedCube.GetComponent<PathVariables>();
    //    generatedCube.transform.parent = path.transform;
    //    pathVariables.entranceLocation = exitLocation;
    //    exitPosition = generatedCube.transform.Find("Exit");
    //    generatedCube.name = pathVariables.entranceLocation + "_" + pathVariables.exitLocation + "_" + generatedCube.name;
    //    lastGeneratedPathType = LocationDecode.PathType.Down;
    //}

    //private void GenerateVerticalCube(GameObject path)
    //{
    //    int exitLocation = pathVariables.exitLocation;
    //    int rand = Random.Range(0, generationVariables.verticalPrefabs.Length);
    //    PathVariables prefabPathVariables = generationVariables.verticalPrefabs[rand].GetComponent<PathVariables>();
    //    GameObject generatedCube = (GameObject)Instantiate(generationVariables.verticalPrefabs[rand], exitPosition.position + exitPosition.forward * prefabPathVariables.sizeOfPath, exitPosition.rotation);
    //    PathVariables currentPathVar = generatedCube.GetComponent<PathVariables>();
    //    currentPathVar.entranceLocation = pathVariables.exitLocation;
    //    currentPathVar.exitLocation = ChooseCloseExit(exitLocation); ;
    //    pathVariables = currentPathVar;
    //    generatedCube.transform.parent = path.transform;
    //    exitPosition = generatedCube.transform.Find("Exit");
    //    generatedCube.name = pathVariables.entranceLocation + "_" + pathVariables.exitLocation + "_" + generatedCube.name;
    //}


    //private void GenerateUpFilpCube(GameObject path)
    //{
    //    int exitLocation = pathVariables.exitLocation;
    //    int rand = Random.Range(0, generationVariables.upFlipPrefabs.Length);
    //    PathVariables prefabPathVariables = generationVariables.upFlipPrefabs[rand].GetComponent<PathVariables>();
    //    GameObject generatedCube = (GameObject)Instantiate(generationVariables.upFlipPrefabs[rand], exitPosition.position + exitPosition.forward * (prefabPathVariables.sizeOfPath), exitPosition.rotation);
    //    if (generationVariables.upDownTurnEnabled)
    //    {
    //        rand = Random.Range(-1, 2);
    //        generatedCube.transform.rotation *= Quaternion.Euler(0, 0, rand * 90);
    //    }
    //    pathVariables = generatedCube.GetComponent<PathVariables>();
    //    generatedCube.name = pathVariables.entranceLocation + "_" + pathVariables.exitLocation + "_" + generatedCube.name;
    //    generatedCube.transform.parent = path.transform;
    //    pathVariables.entranceLocation = exitLocation;
    //    exitPosition = generatedCube.transform.Find("Exit");
    //}

    //private void GenerateDownFilpCube(GameObject path)
    //{
    //    int exitLocation = pathVariables.exitLocation;
    //    int rand = Random.Range(0, generationVariables.downFlipPrefabs.Length);
    //    PathVariables prefabPathVariables = generationVariables.downFlipPrefabs[rand].GetComponent<PathVariables>();
    //    GameObject generatedCube = (GameObject)Instantiate(generationVariables.downFlipPrefabs[rand], exitPosition.position + exitPosition.forward * (prefabPathVariables.sizeOfPath), exitPosition.rotation);
    //    if (generationVariables.upDownTurnEnabled)
    //    {
    //        rand = Random.Range(-1, 2);
    //        generatedCube.transform.rotation *= Quaternion.Euler(0, 0, rand * 90);
    //    }
    //    pathVariables = generatedCube.GetComponent<PathVariables>();
    //    generatedCube.name = pathVariables.entranceLocation + "_" + pathVariables.exitLocation + "_" + generatedCube.name;
    //    generatedCube.transform.parent = path.transform;
    //    pathVariables.entranceLocation = exitLocation;
    //    exitPosition = generatedCube.transform.Find("Exit");
    //}
}
