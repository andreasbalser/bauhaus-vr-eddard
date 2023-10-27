using System.Numerics;
using Unity.Netcode;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

public class ServerMoveAroundTarget : NetworkBehaviour
{
    public Transform target;

    public float degreesPerSecond = 20;

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;
        var newPosition = CalculatePositionUpdate();
        var newRotation = CalculateRotationUpdate(newPosition);
        transform.position = newPosition;
        transform.rotation = newRotation;
    }

    Vector3 CalculatePositionUpdate()
    {
        // Your code for Exercise 1.2 here

        Vector3 translation = transform.position - target.position;
        translation = Quaternion.AngleAxis(Time.deltaTime * degreesPerSecond, Vector3.up) * translation;

        Vector3 newPosition = target.position + translation;

        return newPosition;
    }

    Quaternion CalculateRotationUpdate(Vector3 newPosition)
    {
        // Your code for Exercise 1.2 here

        Vector3 oldPositionVec = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 newPositionVec = new Vector3(newPosition.x, newPosition.y, newPosition.z);
        Vector3 newForward = newPositionVec - oldPositionVec;
        
        Quaternion newRotation = Quaternion.LookRotation(newForward, Vector3.up);

        return newRotation;
    }
}