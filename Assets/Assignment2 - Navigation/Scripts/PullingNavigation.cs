using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PullingNavigation : MonoBehaviour
{
    public InputActionProperty steeringAction;

    public Transform navigationOrigin;
    public Transform steeringHand;

    public float moveSpeed = 60f;
    public float maxSpeed = 1000;
    private float graspThreshhold = 0.2f;

    private Vector3 graspBeginPosition;
    private bool graspActive = false;

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

        if (steeringInput > graspThreshhold)
        {
            if(!graspActive)
            {
                graspBeginPosition = steeringHand.transform.position - navigationOrigin.transform.position;
                graspActive = true;
            }

            Vector3 handPosition = steeringHand.transform.position - navigationOrigin.transform.position;
            
            Vector3 moveDirection = graspBeginPosition - handPosition;
            Debug.Log("Grasp begin: " + graspBeginPosition + "\nHand Position: " + handPosition + "\nMoveDirection: " + moveDirection);
            
            float moveFactor = Math.Min(1 + moveDirection.magnitude * moveSpeed, maxSpeed);
            
            Vector3 moveIncrement = moveDirection * moveFactor;

            navigationOrigin.position += moveIncrement * Time.deltaTime;
        }
        else if (graspActive && steeringInput < graspThreshhold)
        {
            graspActive = false;
            Debug.Log("ran!");
        }
    }
}
