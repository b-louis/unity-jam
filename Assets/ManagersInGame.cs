using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(GameMenuUiController))]
public class ManagersInGame : NetworkBehaviour
{
    public static GameMenuUiController GameUiController;
    public static GameEventsManager GameEvents;
    [SerializeField]
    private PlayerSO _player;
    public static PlayerSO Player;

    private void Awake()
    {
        Player = _player;
        GameEvents = GetComponent<GameEventsManager>();
        GameUiController = GetComponent<GameMenuUiController>();
        Debug.Log(GameUiController);

    }
    void Start()
    {
    }
}
