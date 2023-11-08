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
 private List<GameObject> collidingObjects = new List<GameObject>();
 public List<Collider> restrictedBounds = new List<Collider>();
 public int numObjects = 30;
 public Vector3 generatorBoundsMin = new Vector3(-35, 0, -35);
 public Vector3 generatorBoundsMax = new Vector3(35, 0, 35);
 public bool reset;


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
         GenerateEnvironment();
     }
 }
 
 void ClearEnvironment()
 {
     foreach (var instance in instances)
     {
         Destroy(instance);
     }
     instances.Clear();
 }


 void GenerateEnvironment()
 {
     while (instances.Count < numObjects)
     {
         var prefabIndex = (int)(environmentPrefabs.Count * Random.value);
         GameObject randomObject = Instantiate(environmentPrefabs[prefabIndex]);
         instances.Add(randomObject);
     }
     GenerateObjects(instances);
     
     foreach (var randomObject in instances)
     {
         randomObject.transform.parent = transform;
     }
     
     StartCoroutine(ResolveCollisions());
 }
 

 // GameObject GenerateObjects(GameObject randomObject)
 // {
 //     var randomObjectPosition = new Vector3();
 //       
 //     randomObjectPosition.x = Random.Range(generatorBoundsMin.x, generatorBoundsMax.x);
 //     randomObjectPosition.y = Random.Range(generatorBoundsMin.y, generatorBoundsMax.y);
 //     randomObjectPosition.z = Random.Range(generatorBoundsMin.z, generatorBoundsMax.z);
 //     randomObject.transform.position = randomObjectPosition;
 //     randomObject.transform.rotation = Quaternion.Euler(0, 360 * Random.value, 0);
 //   
 //     return randomObject;
 // }
 
 List<GameObject> GenerateObjects(List<GameObject> randomObjects)
 {
     foreach (var randomObject in randomObjects)
     {
         var randomObjectPosition = new Vector3();

         randomObjectPosition.x = Random.Range(generatorBoundsMin.x, generatorBoundsMax.x);
         randomObjectPosition.y = Random.Range(generatorBoundsMin.y, generatorBoundsMax.y);
         randomObjectPosition.z = Random.Range(generatorBoundsMin.z, generatorBoundsMax.z);
         randomObject.transform.position = randomObjectPosition;
         randomObject.transform.rotation = Quaternion.Euler(0, 360 * Random.value, 0);
     }

     return randomObjects;
 }

 

 // IEnumerator ResolveCollisions()
 // {
 //    yield return new WaitForSeconds(2);
 //    
 //    foreach (var instance in instances)
 //    {
 //         foreach (var bound in restrictedBounds)
 //         {
 //             if(bound.bounds.Intersects(instance.GetComponent<Collider>().bounds))
 //             {
 //                 instances.Remove(instance);
 //                 collidingObjects.Add(instance);
 //             }
 //             
 //         }
 //    }
 //
 //    if (collidingObjects != null)
 //    {
 //        GenerateObjects(collidingObjects);
 //    }
 //
 //     //Run the ResolveCollisions coroutine again for continuous checking
 //     //yield return new WaitForSeconds(2);
 //     StartCoroutine(ResolveCollisions());
 // }

 IEnumerator ResolveCollisions()
 {
     yield return new WaitForSeconds(2);
    
     List<GameObject> objectsToRemove = new List<GameObject>();

     foreach (var instance in instances)
     {
         foreach (var bound in restrictedBounds)
         {
             if (bound.bounds.Intersects(instance.GetComponent<Collider>().bounds))
             {
                 objectsToRemove.Add(instance);
                 collidingObjects.Add(instance);
             }
             else
             {
                 break;
             }
         }
     }

     foreach (var objectToRemove in objectsToRemove)
     {
         instances.Remove(objectToRemove);
     }

     if (collidingObjects.Count > 0)
     {
         GenerateObjects(collidingObjects);
         foreach(var item in collidingObjects)
         {
             instances.Add(item);
         }
     }
 
     StartCoroutine(ResolveCollisions());
 }

 
}