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

public class Relay : MonoBehaviour
{
    public static Relay Singleton;
    private static Dictionary<ulong, PlayerData> clientData ;

    // Start is called before the first frame update
    private void Awake()
    {
        Singleton = this;
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
            clientData = new Dictionary<ulong, PlayerData>();
            clientData[NetworkManager.Singleton.LocalClientId] = new PlayerData(ManagersInGame.Player.name);
            NetworkManager.Singleton.StartHost();
            Debug.Log("Join code is = " + joinCode);
        }catch(RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
    public static async void JoinRelay(string joinCode)
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

            clientData[NetworkManager.Singleton.LocalClientId] = new PlayerData(ManagersInGame.Player.name);
            NetworkManager.Singleton.StartClient();
            //NetworkManager.Singleton.Shutdown();
        }
        catch (RelayServiceException e)
        {
            Debug.LogError(e);
        }
    }
    
    public static PlayerData? GetPlayerData(ulong clientId)
    {
        if (clientData.TryGetValue(clientId, out PlayerData playerData))
        {
            return playerData;
        }

        return null;
    }
}