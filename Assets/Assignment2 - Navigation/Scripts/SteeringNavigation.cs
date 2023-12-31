using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SteeringNavigation : MonoBehaviour
{
    public InputActionProperty steeringAction;

    public Transform navigationOrigin;
    public Transform steeringHand;

    public float moveSpeed = 2f;
    private float moveThreshhold = 0.05f;

    // Start is called before the first frame update
    void Start()
    {
        if (navigationOrigin == null)
        {
            navigationOrigin = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float steeringInput = steeringAction.action.ReadValue<float>();
        // Exercise 2.6 Steering Navigation

        if (steeringInput > moveThreshhold)
        {
            Vector3 moveDirection = steeringHand.forward;
            float moveFactor = moveSpeed * steeringInput;
            
            Vector3 moveIncrement = moveDirection * moveFactor;

            navigationOrigin.position += moveIncrement * Time.deltaTime;
        }
    }
}
