using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Homer : MonoBehaviour
{
    #region Member Variables

    [Header("H.O.M.E.R. Components")] public Transform head;
    public float originHeadOffset = 0.2f;
    public Transform hand;

    [Header("H.O.M.E.R. Parameters")] public LineRenderer ray;
    public float rayMaxLength = 100f;
    public LayerMask layerMask; // use this mask to raycast only for interactable objects

    [Header("Input Actions")] public InputActionProperty grabAction;

    [Header("Grab Configuration")] public HandCollider handCollider;

    // grab calculation variables
    private GameObject grabbedObject;
    private Matrix4x4 offsetMatrix;

    // utility bool to check if you can grab an object
    private bool canGrab
    {
        get
        {
            if (handCollider.isColliding)
                return handCollider.collidingObject.GetComponent<ManipulationSelector>().RequestGrab();
            return false;
        }
    }

    // variables needed for hand offset calculation
    private RaycastHit hit;
    private float grabOffsetDistance;
    private float grabHandDistance;

    // convenience variables for hand offset calculations
    private Vector3 origin
    {
        get
        {
            Vector3 v = head.position;
            v.y -= originHeadOffset;
            return v;
        }
    }

    private Vector3 direction => hand.position - origin;

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        ray.enabled = enabled;
    }

    private void Start()
    {
        if (GetComponentInParent<NetworkObject>() != null)
            if (!GetComponentInParent<NetworkObject>().IsOwner)
            {
                Destroy(this);
                return;
            }

        ray.positionCount = 2;
    }

    private void Update()
    {
        if (grabbedObject == null)
            UpdateRay();
        else
            ApplyHandOffset();

        GrabCalculation();
    }

    #endregion

    #region Custom Methods

    private void UpdateRay()
    {
        //TODO: your solution for excercise 3.5
        // use this function to calculate and adjust the ray of the h.o.m.e.r. technique

        Vector3 handPosition = hand.position;
        ray.SetPosition(0, handPosition);
        ray.SetPosition(1, origin + (direction.normalized * rayMaxLength));
    }

    private void ApplyHandOffset()
    {
        //TODO: your solution for excercise 3.5
        // use this function to calculate and adjust the hand as described in the h.o.m.e.r. technique

        float actualHandDistance = direction.magnitude;
        float distanceMoveRatio = actualHandDistance / grabHandDistance;
        float virtualHandDistance = grabOffsetDistance * distanceMoveRatio;
        
        // move virtual hand
        transform.position = origin + virtualHandDistance * direction.normalized;
    }

    private void GrabCalculation()
    {
        // TODO: your solution for excercise 3.5
        // use this function to calculate the grabbing of an object

        if (grabAction.action.WasPressedThisFrame())
        {
            if (grabbedObject == null && canGrab &&
                Physics.Raycast(hand.position, direction, out hit, rayMaxLength, layerMask))
            {
                grabbedObject = hit.transform.gameObject;
                grabHandDistance = direction.magnitude;
                grabOffsetDistance = hit.distance + grabHandDistance;

                return;
            }
        }

        if (grabAction.action.IsPressed())
        {
            if (grabbedObject != null)
            {
                // TODO: TO BE TESTED!

                // calculate offset matrix
                Matrix4x4 grabbedObjectWorldTransform = grabbedObject.transform.localToWorldMatrix;
                Matrix4x4 handWorldTransform = this.transform.worldToLocalMatrix;
                offsetMatrix = grabbedObjectWorldTransform * handWorldTransform;

                // add offset
                grabbedObject.transform.position =
                    Vector3.Scale(grabbedObject.transform.position, offsetMatrix.GetPosition());
                grabbedObject.transform.rotation = offsetMatrix.rotation;
                grabbedObject.transform.lossyScale.Scale(offsetMatrix.lossyScale);
            }
        }

        else if (grabAction.action.WasReleasedThisFrame())
        {
            if (grabbedObject != null)
                grabbedObject.GetComponent<ManipulationSelector>().Release();
            grabbedObject = null;
            
            this.transform.position = Vector3.zero;
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