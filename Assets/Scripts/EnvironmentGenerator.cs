using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
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
   public bool reset;


   public bool CheckPosition(Vector3 position)
   {
       foreach (var existingObject in instances)
       {
           float minDistance = 10.0f; // Adjustable
           if (Vector3.Distance(existingObject.transform.position, position) < minDistance)
           {
               return false; // Position is too close to an existing object
           }
       }


       // Check if the position is outside of restricted bounds
       foreach (var bound in restrictedBounds)
       {
           if (bound.bounds.Contains(position))
           {
               return false; // Position is inside restricted bounds
           }
       }


       return true; // Position is valid
   }


   // Start is called before the first frame update
   void Start()
   {
       GenerateEnvironment();
   }


   // Update is called once per frame
   void Update()
   {
       if (reset)
       {
           ClearEnvironment();
           reset = false;
       }
   }


   void ClearEnvironment()
   {
       foreach (var instance in instances)
       {
           Destroy(instance);
       }
       instances.Clear();
       GenerateEnvironment();
   }


   void GenerateEnvironment()
   {
       while (instances.Count < numObjects)
       {
           var prefabIndex = (int)(environmentPrefabs.Count * Random.value);
           GameObject randomObject = Instantiate(environmentPrefabs[prefabIndex]);
           //randomObject.transform.parent = transform;


           Vector3 randomObjectPosition = new Vector3();
           bool positionValid = false;


           while (!positionValid)
           {
               randomObjectPosition.x = Random.Range(generatorBoundsMin.x, generatorBoundsMax.x);
               randomObjectPosition.y = 0;
               randomObjectPosition.z = Random.Range(generatorBoundsMin.z, generatorBoundsMax.z);


               positionValid = CheckPosition(randomObjectPosition);
           }
          
         
           randomObject.transform.position = randomObjectPosition;
           randomObject.transform.rotation = Quaternion.Euler(0, 360 * Random.value, 0);
           instances.Add(randomObject);
       }
       foreach (var randomObject in instances)
       {
           randomObject.transform.parent = transform;
       }




       StartCoroutine(ResolveCollisions());
   }


   IEnumerator ResolveCollisions()
   {
       yield return new WaitForSeconds(2);
  
       foreach (var instance in instances)
       {
           foreach (var bound in restrictedBounds)
           {
               if (bound.bounds.Intersects(instance.GetComponent<Collider>().bounds))
               {
                  
                   Vector3 newPosition = new Vector3();
                   newPosition.x = Random.Range(generatorBoundsMin.x, generatorBoundsMax.x);
                   newPosition.z = Random.Range(generatorBoundsMin.z, generatorBoundsMax.z);
                   instance.transform.position = newPosition;
               }
           }
       }
  
       //Run the ResolveCollisions coroutine again for continuous checking
      StartCoroutine(ResolveCollisions());
   }
  
}

