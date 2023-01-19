using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using TMPro;
public class NetworkUI : NetworkBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerCountText;

    private NetworkVariable<int> _count = new NetworkVariable<int>(0, 
        NetworkVariableReadPermission.Everyone, 
        NetworkVariableWritePermission.Server);
    public void OnClickHost()
    {
        NetworkManager.Singleton.StartHost();
    }
    public void OnClickJoin()
    {
        Debug.Log("BO");
        Relay.Singleton.CreateRelay();
    }
    public void OnClickCreateRelay()
    {
        NetworkManager.Singleton.StartClient();
    }

    private void Update()
    {
        playerCountText.text = "Players : "+_count.Value.ToString();
        if (!IsServer) return;
        _count.Value = NetworkManager.Singleton.ConnectedClients.Count;

    }
}
