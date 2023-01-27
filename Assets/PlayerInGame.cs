using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Components;
using Unity.Collections;
using System;

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

    public ulong ownerId;
    // Start is called before the first frame update
    private void OnEnable()
    {
        displayName.OnValueChanged += HandleDisplayNameChanged;
        health.OnValueChanged += HandleHealthChanged;
        alive.OnValueChanged += HandleAliveChanged;

    }

    private void OnDisable()
    {
        displayName.OnValueChanged -= HandleDisplayNameChanged;
        health.OnValueChanged -= HandleHealthChanged;
        alive.OnValueChanged -= HandleAliveChanged;
    }
    void Start()
    {
        //if (!IsOwner) return;
        
    }

    // Update is called once per frame
    public void Hurt(int damage)
    {
        if (!IsOwner) return;
        if (health.Value - damage < 0)
        {
            health.Value = 0;
            alive.Value = false;
            // send message death
        }
        else {
            health.Value -= damage;
        } 
    }
    [ServerRpc(RequireOwnership = false)]
    public void HurtServerRpc(int damage)
    {
        //if (!IsOwner) return;
        Hurt(damage);
    }
    public void SpawnPos(Vector3 pos)
    {
        if (!IsOwner) return;
        transform.position = pos;
        health.Value = 100;
        alive.Value = true;
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
    private IEnumerator waitForData()
    {
        yield return new WaitForSeconds(2f);
        PlayerData playerData = Relay.Singleton.GetPlayerData(OwnerClientId);
        ChangeNameServerRpc(playerData);

    }

    [ServerRpc]
    private void ChangeNameServerRpc(PlayerData playerData)
    {
        displayName.Value = playerData.Username;
    }

    public override void OnNetworkSpawn()
    {
        if (!IsOwner) 
        {
            Debug.Log("I'm no owner !");
            return;
                }
        Debug.LogWarning("My ID is "+NetworkManager.Singleton.LocalClientId);
        Relay.Singleton.setDataServerRpc(NetworkManager.Singleton.LocalClientId);
        Relay.Singleton.PlayerJoinServerRpc();
        StartCoroutine(waitForData());
        health.Value = 100;
        alive.Value = true;
/*        Debug.Log(Relay.Singleton.clientData[0]);
        Debug.Log(Relay.Singleton.clientData[1]);*/
        //RelativeMovement rm = GetComponent<RelativeMovement>();
        transform.position = Relay.Singleton.Spawns[(int)NetworkManager.Singleton.LocalClientId];
        Debug.Log("SPAXN : " + Relay.Singleton.Spawns[(int)NetworkManager.Singleton.LocalClientId]);
        Debug.Log("SPAXN1 : " + Relay.Singleton.Spawns[0]);
        Debug.Log("SPAXN2 : " + Relay.Singleton.Spawns[1]);
        Debug.Log("SPAXN3 : " + (int)NetworkManager.Singleton.LocalClientId);


    }

    private void HandlingDeath()
    {
        // 
        Relay.Singleton.EndingGameServerRpc(NetworkManager.Singleton.LocalClientId);
    }
    private void HandleDisplayNameChanged(FixedString32Bytes oldDisplayName, FixedString32Bytes newDisplayName)
    {
        PlayerText.text = newDisplayName.ToString();
    }
    private void HandleHealthChanged(int oldValue, int newValue)
    {
        PlayerHealth.text = newValue.ToString();
    }
    private void HandleAliveChanged(bool oldValue, bool newValue)
    {
        // Check if we just died, newValue ==> alive.false to alive.true (respawn)
        if (!IsOwner) return;
        if (newValue) return;
        HandlingDeath();
    }

}
