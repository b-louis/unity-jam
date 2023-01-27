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

public class Relay : NetworkBehaviour
{
    public static Relay Singleton;
    public NetworkList<FixedString32Bytes> clientData;
    public NetworkVariable<bool> GameStarted;

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
        DebugLogConsole.AddCommand<string>("joinRelay", "join Realy ", JoinRelay);


        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
         {
             Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
         };

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

    }
    public async void CreateRelay()
    {
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
        }catch(RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
    public async void JoinRelay(string joinCode)
    {
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
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
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
        EndingGameClientRpc(losingClientId);
    }

    [ClientRpc]
    public void EndingGameClientRpc(ulong losingClientId)
    {
        if (NetworkManager.Singleton.LocalClientId == losingClientId)
        {
            Debug.LogWarning("You lose homie ! " + NetworkManager.Singleton.LocalClientId +" "+ losingClientId);
        }
        else
        {
            Debug.LogWarning("Nice play Dawg " + NetworkManager.Singleton.LocalClientId + " " + losingClientId);
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

            StartCoroutine(MyWaitForSec(3));
            // Place Players
            // Unlock Players


        }
    }
    public void PlacePlayer(ulong clientId)
    {
        Debug.LogError("geegeg "+ clientId);

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
        playerinGame.SpawnPos(Spawns[(int)clientId]);
    }
    private IEnumerator MyWaitForSec(int seconds)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log("Game: Ca commence");
    }
}