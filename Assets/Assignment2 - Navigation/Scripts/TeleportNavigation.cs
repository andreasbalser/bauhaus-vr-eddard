using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportNavigation : MonoBehaviour
{
    public InputActionProperty teleportAction;

    public Transform navigationOrigin;
    public Transform head;
    public Transform hand;
    
    public LayerMask groundLayerMask;

    public GameObject previewAvatar;
    public GameObject hitpoint;

    public GameObject navigationPlatformGeometry;

    public float rayLength = 20.0f;
    private bool rayIsActive = false;

    public XRInteractorLineVisual lineVisual;
    private float rayActivationThreshhold = 0.01f;
    private float teleportActivationThreshhold = 0.5f;
    
    // Added parameters

    private bool teleportPositionIsSet;

    private RaycastHit hit;
    private Vector3 teleportPosition = Vector3.zero;
    private Quaternion teleportRotation = Quaternion.identity;

    


    // Start is called before the first frame update
    void Start()
    {
        lineVisual.enabled = false;
        hitpoint.SetActive(false);
        previewAvatar.SetActive(false);
        
        
    }

    // Update is called once per frame
    void Update()
    {
        // activate line
        float teleportActionValue = teleportAction.action.ReadValue<float>();
        if (teleportActionValue > rayActivationThreshhold && !rayIsActive)
        {
            rayIsActive = true;
            lineVisual.enabled = rayIsActive;
            hitpoint.SetActive(true); // show
        }
        else if (teleportActionValue < rayActivationThreshhold && rayIsActive)
        {
            rayIsActive = false;
            lineVisual.enabled = rayIsActive;
            hitpoint.SetActive(false); // hide
        }

        // Exercise 2.8 Teleport Navigation
        
        if(rayIsActive) SetHitpointPosition();

        if (teleportActionValue > teleportActivationThreshhold && rayIsActive)
        {
            if (!teleportPositionIsSet) SetTeleportPosition();
            UpdateTeleportRotation();
        }
        else if (teleportActionValue < teleportActivationThreshhold && teleportPositionIsSet)
        {
            PerformTeleport();
        }
    }

    void SetHitpointPosition()
    {
        LayerMask ignoreHitpoint = ~(1 << hitpoint.layer);
        if (Physics.Raycast(hand.position, hand.forward, out hit, rayLength, groundLayerMask))
        {
            hitpoint.transform.position = hit.point;
        }
    }

    void SetTeleportPosition()
    {
        // set teleport position
        teleportPosition = hit.point;
        teleportPositionIsSet = true;

        previewAvatar.SetActive(true);
        
        // set PreviewAvatar position
        Vector3 heightOffset = new Vector3(0, head.transform.localPosition.y, 0);
        Vector3 previewAvatarPosition = hit.point + heightOffset;
        previewAvatar.transform.position = previewAvatarPosition;
    }

    void UpdateTeleportRotation()
    {
        Vector3 teleportForwardDirection = teleportPosition - hitpoint.transform.position;
        teleportForwardDirection.y = 0;
        teleportRotation = Quaternion.LookRotation(teleportForwardDirection);
        
        // Update PreviewAvatar rotation
        previewAvatar.transform.rotation = teleportRotation;
    }

    void PerformTeleport()
    {
        // Perform Teleport
        Vector3 headPositionWithoutY = new Vector3(head.transform.localPosition.x, 0, head.transform.localPosition.z);
        float headRotationY = head.transform.localRotation.eulerAngles.y;
        float teleportRotationY = teleportRotation.eulerAngles.y;
        Debug.Log("headRotationY: " + headRotationY + " , teleportRotationY: " + teleportRotationY +"\n OffsetAngle: " + (teleportRotationY + headRotationY)%360);
        Quaternion teleportRotationOffset = Quaternion.Euler(0, -teleportRotationY + headRotationY + 180, 0);

        Matrix4x4 teleportTransform =
            Matrix4x4.Translate(teleportPosition) *
            Matrix4x4.Inverse(Matrix4x4.TRS(headPositionWithoutY, teleportRotationOffset, head.transform.localScale));
        
        
        navigationOrigin.transform.position = teleportTransform.GetColumn(3);
        navigationOrigin.transform.rotation = teleportTransform.rotation;
        navigationOrigin.transform.localScale = teleportTransform.lossyScale;
        
        // Clear teleport info
        teleportPosition = Vector3.zero;
        teleportRotation = Quaternion.identity;
        teleportPositionIsSet = false;

        // Hide PreviewAvatar
        previewAvatar.SetActive(false);
        
        Debug.Log("performed Teleport");
    }
}   
