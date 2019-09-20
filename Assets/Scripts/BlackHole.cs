using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    [SerializeField] MapGenerator mapGen;
    
    // Movement speed in units per second.
    public float speed = 1.0F;

    // Update is called once per frame
    void Update()
    {
        if (mapGen.lastGeneratedTurn.Count != 0)
        {
            //use movetowards
            transform.position = Vector3.MoveTowards(transform.position, mapGen.lastGeneratedTurn.Peek().pos, Time.deltaTime * speed);

            if (Vector3.Distance(transform.position, mapGen.lastGeneratedTurn.Peek().pos) <= 10)
            {
                transform.position = mapGen.lastGeneratedTurn.Peek().pos;
                transform.rotation = mapGen.lastGeneratedTurn.Peek().rot;
                mapGen.lastGeneratedTurn.Dequeue();
            }
        }

    }

}

