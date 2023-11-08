using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnvironmentGenerator : MonoBehaviour
{
     public List<GameObject> environmentPrefabs = new List<GameObject>();
     private List<GameObject> instances = new List<GameObject>();
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
             GenerateEnvironment();
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
     }


     void GenerateEnvironment()
     {
         while (instances.Count < numObjects)
         {
             var prefabIndex = (int)(environmentPrefabs.Count * Random.value);
             GameObject randomObject = Instantiate(environmentPrefabs[prefabIndex]);
             instances.Add(randomObject);
         }
         PlaceObjectsRandomly(instances);
         
         foreach (var randomObject in instances)
         {
             randomObject.transform.parent = transform;
         }
         
         StartCoroutine(ResolveCollisions());
     }
     
     List<GameObject> PlaceObjectsRandomly(List<GameObject> objects)
     {
         foreach (var gameObject in objects)
         {
             gameObject.transform.position = GetRandomVectorInBounds(generatorBoundsMin, generatorBoundsMax);
             gameObject.transform.rotation = GetRandomRotation(false, true, false);
         }

         return objects;
     }

     Vector3 GetRandomVectorInBounds(Vector3 boundsMin, Vector3 boundsMax)
     {
         Vector3 randomVector = Vector3.zero;

         randomVector.x = Random.Range(boundsMin.x, boundsMax.x);
         randomVector.y = Random.Range(boundsMin.y, boundsMax.y);
         randomVector.z = Random.Range(boundsMin.z, boundsMax.z);

         return randomVector;
     }

     Quaternion GetRandomRotation(bool rotateX, bool rotateY, bool rotateZ)
     {
         float randomX = rotateX ? 360 * Random.value : 0;
         float randomY = rotateY ? 360 * Random.value : 0;
         float randomZ = rotateZ ? 360 * Random.value : 0;
         
         Quaternion randomRotation = Quaternion.Euler(randomX, randomY, randomZ);

         return randomRotation;
     }

     IEnumerator ResolveCollisions()
     {
         yield return new WaitForSeconds(2);

         bool checkAgain = false;

         foreach (var instance in instances)
         {
             Collider instanceCollider = instance.GetComponent<Collider>();
             
             foreach (var bound in restrictedBounds)
             {
                 if (bound.bounds.Intersects(instanceCollider.bounds))
                 {
                     instance.transform.position = GetRandomVectorInBounds(generatorBoundsMin, generatorBoundsMax);
                     instance.transform.rotation = GetRandomRotation(false, true, false);

                     checkAgain = true;
                 }
             }
         }

         if (checkAgain)
         {
             Debug.Log("Checking Collisions Again");
             StartCoroutine(ResolveCollisions());
         }
         else
         {
             Debug.Log("Checking done!");
         }
     }
}