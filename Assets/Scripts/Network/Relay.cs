using System.Collections;
using System.Collections.Generic;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using IngameDebugConsole;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Collections;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;

public class Relay : NetworkBehaviour
{
    public static Relay Singleton;
    public NetworkList<FixedString32Bytes> clientData;
    public NetworkVariable<bool> GameStarted;
    public NetworkVariable<bool> GameIsReady;
    public GameObject EndGame;
    public TextMeshProUGUI TextEnd;
    public List<Vector3> Spawns;
    // Start is called before the first frame update
    private void Awake()
    {
        Singleton = this;
        clientData = new NetworkList<FixedString32Bytes>();
        GameStarted = new NetworkVariable<bool>(false);
        Spawns.Add(new Vector3(8, 1, 0));
        Spawns.Add(new Vector3(-8, 1, 0));
        //NetworkManager.Singleton.OnClientConnectedCallback += setDataServerRpc;

    }
    private void OnEnable()
    {
        
    }
    private async void Start()
    {
        DebugLogConsole.AddCommand("createRelay", "creates Realy ", CreateRelay);
        //DebugLogConsole.AddCommand<string>("joinRelay", "join Realy ", JoinRelay);


        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
         {
             Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
         };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }
    public async UniTask<string> CreateRelay()
    {
        if(Managers.Player.Balance < 10)
        {
            return "N";
        }
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(1);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            Debug.Log("Join code is = " + joinCode);
            Debug.Log(clientData);
            Debug.Log(OwnerClientId);
            Debug.Log(NetworkManager.Singleton.LocalClientId);
            Debug.Log((int)NetworkManager.Singleton.LocalClientId);
            NetworkManager.Singleton.StartHost();
            setDataServerRpc(OwnerClientId);
            Debug.Log(clientData.Count); 
            Debug.Log("Join code is = " + joinCode);
            return joinCode;
        }catch(RelayServiceException e)
        {
            Debug.LogError(e);
            return "";

        }
    }
    public async UniTask<int> JoinRelay(string joinCode)
    {
        if (Managers.Player.Balance < 10)
        {
            return 2;
        }
        try
        {
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData
            );

            NetworkManager.Singleton.StartClient();
            //NetworkManager.Singleton.Shutdown();
            return 0;
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
            return 1;
        }
    }
    
    public void Leaving()
    {

        Destroy(NetworkManager.gameObject);
        Managers.GameSceneManager.GoMenu();
    }
    [ServerRpc(RequireOwnership = false)]
    public void setDataServerRpc(ulong clientId)
    {
        if (IsServer && IsOwner)
        {
            if (clientData.Count < 1)
            {
                clientData.Add("Player1");
                clientData.Add("Player2");


            }
        }
        if (IsOwner)
        {
            Debug.Log("My name");
            clientData[(int)clientId] = clientId.ToString();
        }
        Debug.Log(clientData[0]);
        Debug.Log(clientData[1]);

    }

    public PlayerData GetPlayerData(ulong clientId)
    {
        Debug.Log(clientData.Count);
        Debug.Log(clientData[(int)clientId]);

        return new PlayerData(clientData[(int)clientId].ToString());
    }

    [ServerRpc(RequireOwnership =false)]
    public void EndingGameServerRpc(ulong losingClientId) 
    {
        GameStarted.Value = false;
        EndingGameClientRpc(losingClientId);
    }
    private IEnumerator DelayGameEnd(int seconds)
    {

        yield return new WaitForSeconds(seconds);
        Debug.Log("Game: Ca fini");
        NetworkManager.Singleton.Shutdown();
        Leaving();
    }

    [ClientRpc]
    public void EndingGameClientRpc(ulong losingClientId)
    {
        //
        EndGame.SetActive(true);
        if (NetworkManager.Singleton.LocalClientId == losingClientId)
        {
            Debug.LogWarning("You lose homie ! " + NetworkManager.Singleton.LocalClientId +" "+ losingClientId);
            TextEnd.text = "You lose !";
            StartCoroutine(DelayGameEnd(3));
        }
        else
        {
            Debug.LogWarning("Nice play Dawg " + NetworkManager.Singleton.LocalClientId + " " + losingClientId);
            TextEnd.text = "You win !";
            Win();
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void PlayerJoinServerRpc()
    {
        Debug.Log("Game: Player joins");

        if (NetworkManager.Singleton.ConnectedClients.Count == 2)
        {
            // We start the game :
            Debug.Log("Game: Ca va commencer");
            GameIsReady.Value = true;
            //PayClientRpc();
            StartCoroutine(DelayGameStart(3));
            // Place Players
            // Unlock Players


        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void RematchServerRpc()
    {

        PlacePlayer(0);
        PlacePlayer(1);
        GameStarted.Value = true;
    }
    public void PlacePlayer(ulong clientId)
    {

        if (!NetworkManager.Singleton.ConnectedClients.TryGetValue(clientId, out NetworkClient networkClient))
        {
            return;
        }

        // Try to get the TeamPlayer component from the player object
        // Return if unsuccessful
        if (!networkClient.PlayerObject.TryGetComponent<PlayerInGame>(out var playerinGame))
        {
            return;
        }

        // Send a message to the server to set the local client's team
        playerinGame.SpawnClientRpc();
    }
    [ClientRpc]
    private void PayClientRpc()
    {
        _ = Managers.Metafab.TransfertCurrensyPlay();

    }
    private async void Win()
    {
        await Managers.Metafab.TransfertCurrensyWin();
        StartCoroutine(DelayGameEnd(2));

    }
    private IEnumerator DelayGameStart(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log("Game: Ca commence");
        RematchServerRpc();
    }
}