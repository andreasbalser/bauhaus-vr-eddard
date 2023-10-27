using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentGenerator : MonoBehaviour
{
    public List<GameObject> environmentPrefabs = new List<GameObject>();

    private List<GameObject> instances = new List<GameObject>();

    public List<Collider> restrictedBounds = new List<Collider>();

    public int numObjects = 30;

    public Vector3 generatorBoundsMin = new Vector3(-30, 0, -30);

    public Vector3 generatorBoundsMax = new Vector3(30, 0, 30);

    public bool reset = false;

    // Start is called before the first frame update
    void Start()
    {
        // Your code for Exercise 1.1 part 1.) here
        GenerateEnvironment();
    }

    // Update is called once per frame
    void Update()
    {
        // Your code for Exercise 1.1 part 3.) here
    }

    void ClearEnvironment()
    {
        // Your code for Exercise 1.1 part 3.) here
    }

    void GenerateEnvironment()
    {
        // Your code for Exercise 1.1 part 1.) here
        
        while (instances.Count < numObjects)
        {
            GameObject randomObject =
                Instantiate(environmentPrefabs.ElementAt(Convert.ToInt32(environmentPrefabs.Count * Random.value)));
            
            randomObject.transform.parent = this.gameObject.transform;
            
            Vector3 tmp = new Vector3();
            tmp.x = Random.Range(generatorBoundsMin.x, generatorBoundsMax.x);
            tmp.y = Random.Range(generatorBoundsMin.y, generatorBoundsMax.y);
            tmp.z = Random.Range(generatorBoundsMin.z, generatorBoundsMax.z);
            
            randomObject.transform.position = tmp;
            
            Debug.Log(tmp);
            
            //randomObject.transform.position = new Vector3(randomObject.transform.position.x, 0, randomObject.transform.position.z);
            randomObject.transform.Rotate(new Vector3(0, 360 * Random.value, 0));
    
            instances.Add(randomObject);
        }
        
        StartCoroutine(ResolveCollisions());

        
    }

    IEnumerator ResolveCollisions()
    {
        yield return new WaitForSeconds(2);
        bool resolveAgain = false;
        // Your code for Exercise 1.1 part 2.) here
        if (resolveAgain)
            StartCoroutine(ResolveCollisions());
    }
}
