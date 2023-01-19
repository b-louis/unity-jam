using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(PlayerManagerNetwork))]
public class ManagersInGame : NetworkBehaviour
{
    public static PlayerManagerNetwork PlayerNetwork;
    public static GameEventsManager GameEvents;
    private void Awake()
    {
        GameEvents = GetComponent<GameEventsManager>();
        PlayerNetwork = GetComponent<PlayerManagerNetwork>();
        Debug.Log(PlayerNetwork);

    }
    void Start()
    {
    }
}
