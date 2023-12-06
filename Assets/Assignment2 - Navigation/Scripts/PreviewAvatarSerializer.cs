using Unity.Netcode;
using UnityEngine;

public class PreviewAvatarSerializer : NetworkBehaviour
{
    public GameObject previewAvatar;
    private NetworkVariable<bool> avatarIsVisible = new NetworkVariable<bool>(default, writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<Vector3> avatarPosition = new NetworkVariable<Vector3>(default, writePerm: NetworkVariableWritePermission.Owner);
    private NetworkVariable<float> avatarRotation = new NetworkVariable<float>(default, writePerm: NetworkVariableWritePermission.Owner);

    public override void OnNetworkSpawn()
    {
        previewAvatar.SetActive(false);

        if (IsOwner)
            return;

        ApplyAvatarUpdates();
    }
    
    // runs in case IsOwner: Writes values from own scene to the Network Variables
    private bool SerializeAvatarUpdates(out bool avatarVisible, out Vector3 avatarPos, out float avatarRot)
    {
        avatarVisible = false;
        avatarPos = previewAvatar.transform.position;
        avatarRot = previewAvatar.transform.rotation.eulerAngles.y;
        if (previewAvatar.activeSelf)
        {
            avatarVisible = true;
            return true;
        }
        else if (!previewAvatar.activeSelf && avatarIsVisible.Value)
        {
            avatarVisible = false;
            return true;
        }

        return false;
    }

    // runs in case !IsOwner: Applies values of Network Variables to the own scene
    private void ApplyAvatarUpdates()
    {
        previewAvatar.SetActive(avatarIsVisible.Value);

        if (previewAvatar.activeSelf)
        {
            previewAvatar.transform.position = avatarPosition.Value;
            previewAvatar.transform.rotation = Quaternion.Euler(0, avatarRotation.Value, 0);
        }
    }


    private void Update()
    {
        if (IsOwner)
        {
            if (SerializeAvatarUpdates(out bool avatarVisible, out Vector3 avatarPos, out float avatarRot))
            {
                avatarIsVisible.Value = avatarVisible;
                avatarPosition.Value = avatarPos;
                avatarRotation.Value = avatarRot;
            }
        }
        else if (!IsOwner)
        {
            ApplyAvatarUpdates();
        }
    }

}
