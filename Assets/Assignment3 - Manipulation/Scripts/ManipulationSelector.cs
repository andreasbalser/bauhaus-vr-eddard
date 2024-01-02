using Unity.Netcode;

public class ManipulationSelector : NetworkBehaviour
{
    #region Member Variables

    private NetworkVariable<bool> isGrabbed = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server);

    #endregion

    #region Selector Methods

    public bool RequestGrab()
    {
        // TODO: your solution for excercise 3.8
        // check if object can be grabbed by a user
        if (isGrabbed.Value) return false;
        
        // trigger ownership handling
        transferOwnershipServerRpc();
        
        // trigger grabbed state update
        updateGrabbedStateServerRpc(true);

        return GetComponent<NetworkObject>().IsOwner;
    }

    public void Release()
    {
        // TODO: your solution for excercise 3.8
        // use this function trigger a grabbed state update on object release
        updateGrabbedStateServerRpc(false);
    }

    #endregion

    #region RPCs

    // TODO: your solution for excercise 3.8
    // implement a rpc to transfer the ownership of an object 
    // implement a rpc to update the isGrabbed value

    [ServerRpc (RequireOwnership = false)]
    void transferOwnershipServerRpc(ServerRpcParams serverRpcParams = default)
    {
        var clientId = serverRpcParams.Receive.SenderClientId;
        
        GetComponent<NetworkObject>().ChangeOwnership(clientId);
    }

    [ServerRpc]
    void updateGrabbedStateServerRpc(bool value)
    {
        isGrabbed.Value = value;
    }

    #endregion
}
