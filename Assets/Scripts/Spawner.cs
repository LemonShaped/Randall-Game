using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject objectToSpawn;
    public float timePerSpawn;

    public float timeRemainingUntilSpawn;


    // Update is called once per frame
    void Update()
    {
        if (timeRemainingUntilSpawn <= 0) {
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
            timeRemainingUntilSpawn = timePerSpawn;
        }
        else
            timeRemainingUntilSpawn -= Time.deltaTime;

    }
}
