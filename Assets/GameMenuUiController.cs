using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using IngameDebugConsole;
using TMPro;
using Unity.Collections;
using UnityEngine.UI;
using System;

public class GameMenuUiController : MonoBehaviour
{
    public TMP_InputField RelayCode;
    public GameObject Main;
    public GameObject CountDown;
    public GameObject Endgame;
    public TextMeshProUGUI RelayCodeToFriend;
    public Button CreateParty;
    public Button JoinParty;

    private void OnEnable()
    {
        Relay.Singleton.GameStarted.OnValueChanged += GameStartedHandleChange;
        Relay.Singleton.GameIsReady.OnValueChanged += GameIsReadyHandleChange;
    }

    private void OnDisable()
    {
        Relay.Singleton.GameStarted.OnValueChanged -= GameStartedHandleChange;
        Relay.Singleton.GameIsReady.OnValueChanged -= GameIsReadyHandleChange;
    }
    private void GameStartedHandleChange(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            Main.SetActive(false);
        }
        if (!newValue)
        {
            Main.SetActive(Endgame);
        }
    }
    private void GameIsReadyHandleChange(bool previousValue, bool newValue)
    {
        if (newValue)
        {
            CountDown.SetActive(true);
        }
    }
    private void Start()
    {
    }
    public async void OnPlayerCreate()
    {
        CreateParty.interactable = false;
        JoinParty.interactable = false;
        string response = await Relay.Singleton.CreateRelay();
        if (response.Length == 0)
        {
            CreateParty.interactable = true;
            JoinParty.interactable = true;
            RelayCodeToFriend.text = "Cant create a party !";
            Debug.LogError("Cant create a party !");
        }else if (response.Length == 1)
        {
            RelayCodeToFriend.text = "Not enough balance to play !";
            CreateParty.interactable = true;
            JoinParty.interactable = true;
        }
        else
        {
            RelayCodeToFriend.text = response;
            //Main.SetActive(false);
        }

    }
    public async void OnPlayerJoin()
    {
        if (RelayCode.text.Length == 0)
        {
            return;
        }
        CreateParty.interactable = false;
        JoinParty.interactable = false;
        int response = await Relay.Singleton.JoinRelay(RelayCode.text);
        if (response == 1)
        {
            CreateParty.interactable = true;
            JoinParty.interactable = true;
            RelayCodeToFriend.text = "Cant join party !";
            Debug.LogError("Cant join party !");
        }else if(response == 2)
        {
            RelayCodeToFriend.text = "Not enough balance to play !";
            CreateParty.interactable = true;
            JoinParty.interactable = true;
        }
        else
        {
            CreateParty.interactable = false;
            JoinParty.interactable = false;

        }
    }


}
