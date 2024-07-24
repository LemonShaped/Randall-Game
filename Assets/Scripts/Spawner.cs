using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameManager gameManager;

    public GameObject objectToSpawn;
    public Vector2 offset;

    public Transform parentObject;

    public float timePerSpawn;

    public float timeRemainingUntilSpawn;
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if (timeRemainingUntilSpawn <= 0) {
            Instantiate(objectToSpawn, transform.position + (Vector3)offset, Quaternion.identity, parentObject);
            timeRemainingUntilSpawn = timePerSpawn;
        }
        else
            timeRemainingUntilSpawn -= Time.deltaTime;

    }
}
