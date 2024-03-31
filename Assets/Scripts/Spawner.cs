using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{

    public GameObject objectToSpawn;
    public float timePerSpawn;

    public float timeSinceLastSpawn;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastSpawn += Time.deltaTime;
        if (timeSinceLastSpawn >= timePerSpawn) {
            timeSinceLastSpawn = 0;
            Instantiate(objectToSpawn, transform.position, Quaternion.identity);
        }

    }
}
