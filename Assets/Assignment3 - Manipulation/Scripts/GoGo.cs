using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GoGo : MonoBehaviour
{
    #region Member Variables

    [Header("Go-Go Components")] 
    public Transform head;
    public float originHeadOffset = 0.2f;
    public Transform hand;

    [Header("Go-Go Parameters")] 
    public float distanceThreshold;
    [Range(0, 1)] public float k;
    
    [Header("Input Actions")] 
    public InputActionProperty grabAction;
    
    [Header("Grab Configuration")]
    public HandCollider handCollider;
    
    // calculation variables
    private GameObject grabbedObject;
    private Matrix4x4 offsetMatrix;
    
    private bool canGrab
    {
        get
        {
            if (handCollider.isColliding)
                return handCollider.collidingObject.GetComponent<ManipulationSelector>().RequestGrab();
            return false;
        }
    }
    
    private Vector3 origin
    {
        get
        {
            Vector3 v = head.position;
            v.y -= originHeadOffset;
            return v;
        }
    }

    private Vector3 handDirection => hand.position - origin;
    private float handDistance => handDirection.magnitude;

    #endregion

    #region MonoBehaviour Callbacks

    private void Start()
    {
        if(GetComponentInParent<NetworkObject>() != null)
            if (!GetComponentInParent<NetworkObject>().IsOwner)
            {
                Destroy(this);
                return;
            }
    }

    private void Update()
    {
        ApplyHandOffset();
        GrabCalculation();
    }

    #endregion

    #region Custom Methods

    private void ApplyHandOffset()
    {
        // TODO: your solution for excercise 3.6
        // use this function to calculate and apply the hand displacement according to the go-go technique

        float thresholdOvershoot = handDistance - distanceThreshold;
        
        if (thresholdOvershoot > 0)
        {
            float virtualHandDistance = handDistance + k * (float)Math.Pow(thresholdOvershoot, 2);

            this.transform.position = origin + virtualHandDistance * handDirection.normalized;
        }
    }

    private void GrabCalculation()
    {
        // TODO: your solution for excercise 3.6
        // use this function to calculate the grabbing of an object
        
        if (grabAction.action.IsPressed())
        {
            if (grabbedObject == null && canGrab)
            {
                grabbedObject = handCollider.collidingObject;
            }

            if (grabbedObject != null)
            {
                // TODO: TO BE TESTED!
                
                Matrix4x4 grabbedObjectWorldTransform = grabbedObject.transform.localToWorldMatrix;
                Matrix4x4 handWorldTransform = transform.worldToLocalMatrix;
                offsetMatrix = grabbedObjectWorldTransform * handWorldTransform;

                grabbedObject.transform.position = Vector3.Scale(grabbedObject.transform.position, offsetMatrix.GetPosition());
                grabbedObject.transform.rotation = offsetMatrix.rotation;
                grabbedObject.transform.lossyScale.Scale(offsetMatrix.lossyScale);
            }
        }
        else if (grabAction.action.WasReleasedThisFrame())
        {
            if(grabbedObject != null)
                grabbedObject.GetComponent<ManipulationSelector>().Release();
            grabbedObject = null;
        }
    }

    #endregion

    #region Utility Functions

    public Matrix4x4 GetTransformationMatrix(Transform t, bool inWorldSpace = true)
    {
        if (inWorldSpace)
        {
            return Matrix4x4.TRS(t.position, t.rotation, t.lossyScale);
        }
        else
        {
            return Matrix4x4.TRS(t.localPosition, t.localRotation, t.localScale);
        }
    }

    #endregion
}
