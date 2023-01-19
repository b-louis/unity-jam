using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using IngameDebugConsole;
using TMPro;
using Unity.Collections;

public class PlayerManagerNetwork : NetworkBehaviour
{
    public TMP_InputField RelayCode;

    private void Start()
    {
        Debug.Log("Player Start");
    }
    public void OnPlayerDeath()
    {
    }
    public void OnPlayerJoin()
    {
        Relay.Singleton.JoinRelay(RelayCode.text);
    }
    [ServerRpc]
    public void PlayerJoinServerRpc(PlayerSerializedData player)
    {
        Debug.Log(player.username + " has join");
    }
    public void PlayerLeave(PlayerInGame player)
    {
    }
    public void GetPlayerNames()
    {
    }
}
