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

    private RaycastHit raycastHit;
    private Vector3 teleportPosition = Vector3.zero;
    private Quaternion avatarRotation = Quaternion.identity;
    


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
            if (!teleportPositionIsSet)
            {
                SetTeleportPosition();
                ShowPreviewAvatar();
            }
            UpdateTeleportRotation();
            UpdatePreviewAvatar();
        }
        else if (teleportActionValue < teleportActivationThreshhold && teleportPositionIsSet)
        {
            PerformTeleport();
        }
    }

    void SetHitpointPosition()
    {
        if (Physics.Raycast(hand.position, hand.forward, out raycastHit, rayLength, groundLayerMask))
        {
            hitpoint.transform.position = raycastHit.point;
        }
    }

    void SetTeleportPosition()
    {
        // set teleport position
        teleportPosition = raycastHit.point;
        teleportPositionIsSet = true;
        
        ShowPreviewAvatar();
        
        Debug.Log("locked teleport position");
    }

    void ShowPreviewAvatar()
    {
        if (!teleportPositionIsSet) return;
        
        Vector3 heightOffset = new Vector3(0, head.transform.position.y - navigationOrigin.transform.position.y, 0);
        Vector3 previewAvatarPosition = raycastHit.point + heightOffset;
        previewAvatar.transform.position = previewAvatarPosition;
        
        previewAvatar.SetActive(true);
    }

    void UpdateTeleportRotation()
    {
        // Calculate teleport rotation
        Vector3 teleportForwardDirection = teleportPosition - hitpoint.transform.position;
        teleportForwardDirection.y = 0;
        avatarRotation = Quaternion.LookRotation(teleportForwardDirection, Vector3.up);
    }

    void UpdatePreviewAvatar()
    {
        Transform avatarTransform = previewAvatar.transform;

        // Update rotation
        avatarTransform.rotation = avatarRotation;
        
        // Update height
        float heightOffset = head.transform.position.y - navigationOrigin.transform.position.y;
        avatarTransform.position = new Vector3(avatarTransform.position.x, teleportPosition.y + heightOffset, avatarTransform.position.z);
    }

    void PerformTeleport()
    {
        // for the teleport transform, we don't want the y-coordinate of the head
        Vector3 headXZPosition = head.localPosition;
        headXZPosition.y = 0;
        
        // calculate teleport rotation
        float headRotationY = head.localRotation.eulerAngles.y;
        float avatarRotationY = avatarRotation.eulerAngles.y;
        float teleportRotationAngle = headRotationY - avatarRotationY + 180;
        Quaternion teleportRotation = Quaternion.Euler(0, teleportRotationAngle, 0);
        
        
        Matrix4x4 teleportTransform =
            Matrix4x4.Translate(teleportPosition) *
            Matrix4x4.Inverse(Matrix4x4.TRS(headXZPosition, teleportRotation, head.transform.localScale));
        
        // performing the teleport
        Transform userTransform = navigationOrigin.transform;
        userTransform.position = teleportTransform.GetColumn(3);
        userTransform.rotation = teleportTransform.rotation;
        userTransform.localScale = teleportTransform.lossyScale;
        
        Debug.Log("performed Teleport");
        
        // Clear teleport info
        teleportPosition = Vector3.zero;
        avatarRotation = Quaternion.identity;
        teleportPositionIsSet = false;

        // Hide PreviewAvatar
        previewAvatar.SetActive(false);
    }
}   
