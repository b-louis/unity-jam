using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Collections;

public struct PlayerSerializedData : INetworkSerializable
{
    public FixedString32Bytes username;
    public FixedString512Bytes accesToken;

    public PlayerSerializedData(string username, string accesToken)
    {
        this.username = username;
        this.accesToken = accesToken;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref username);
        serializer.SerializeValue(ref accesToken);

    }
}
public class PlayerInGame : NetworkBehaviour
{


    public PlayerSO PlayerData;
    public TextMeshProUGUI PlayerText;
    public TextMeshProUGUI PlayerHealth;

    public NetworkVariable<int> health = new NetworkVariable<int>(100);
    public NetworkVariable<bool> alive = new NetworkVariable<bool>(true);
    public NetworkVariable<PlayerSerializedData> PlayerDataS;
    private NetworkVariable<FixedString32Bytes> displayName = new NetworkVariable<FixedString32Bytes>();

    //public NetworkVariable<float> health = new NetworkVariable<float>(100f);

    public int ownerId;
    // Start is called before the first frame update
    void Start()
    {
        //if (!IsOwner) return;

    }

    // Update is called once per frame
    public void Hurt(int damage)
    {
        float health_value = health.Value;
        //if (!IsOwner) return;
        if (health_value - damage < 0)
        {
            health.Value = 0;
            alive.Value = false;
            // send message death
        }
        else {
            health.Value -= damage;
            PlayerHealth.text = health.Value.ToString();
        } 
    }
    [ServerRpc]
    public void HurtServerRpc(int damage)
    {
        if (!IsOwner) return;
        Hurt(damage);
    }
    public void Spawn()
    {
        if (!IsOwner) return;
        health.Value = 100;
        alive.Value = true;
    }
    [ServerRpc]
    public void SpawnServerRpc()
    {
        Spawn();
    }
    public override void OnNetworkSpawn()
    {
        if (!IsServer) { return; }

        PlayerData? playerData = Relay.GetPlayerData(OwnerClientId);

        if (playerData.HasValue)
        {
            displayName.Value = playerData.Value.Username;
        }

        PlayerText.text = PlayerDataS.Value.username.ToString();
        health.Value = 100;
        alive.Value = true;
    }

    
}
