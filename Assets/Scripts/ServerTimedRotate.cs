using System;
using Unity.Netcode;
using UnityEngine;

public class ServerTimedRotate : NetworkBehaviour
{
    public float degreesPerSecondX = 0;
    public float degreesPerSecondY = 20;
    public float degreesPerSecondZ = 0;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer) return;
        
        // Your code for Exercise 1.4 here 
        
        float deltaTime = Time.deltaTime;
        
        Quaternion addedRotation = Quaternion.Euler(deltaTime * degreesPerSecondX, deltaTime * degreesPerSecondY,
            deltaTime * degreesPerSecondZ);

        transform.localRotation = transform.localRotation * addedRotation;
    }
}
