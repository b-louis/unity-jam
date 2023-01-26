using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using IngameDebugConsole;
using TMPro;
using Unity.Collections;

public class GameMenuUiController : MonoBehaviour
{
    public TMP_InputField RelayCode;
    private void Start()
    {
    }
    public void OnPlayerDeath()
    {
    }
    public void OnPlayerJoin()
    {
        Relay.JoinRelay(RelayCode.text);
    }

}
