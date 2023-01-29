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
    public NetworkVariable<int> health = new NetworkVariable<int>(10);
    public NetworkVariable<bool> alive = new NetworkVariable<bool>(false);
    public NetworkVariable<PlayerSerializedData> PlayerDataS;
    private NetworkVariable<FixedString32Bytes> displayName = new NetworkVariable<FixedString32Bytes>();
    private RelativeMovement rm;
    //public NetworkVariable<float> health = new NetworkVariable<float>(100f);

    public ulong ownerId;
    // Start is called before the first frame update
    private void OnEnable()
    {
        displayName.OnValueChanged += HandleDisplayNameChanged;
        health.OnValueChanged += HandleHealthChanged;
        alive.OnValueChanged += HandleAliveChanged;

        rm = GetComponent<RelativeMovement>();



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
        if (health.Value - damage <= 0)
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
    [ClientRpc]
    public void SpawnClientRpc()
    {
        /*        if (!IsOwner) return;
        */
        SpawnServerRpc();
        PlayerText.text = "You";
        var Renderer = GetComponentInChildren<Renderer>();

        // Call SetColor using the shader property name "_Color" and setting the color to red
        Renderer.material.SetColor("_Color", Color.green);
        //PlayerText.text = displayName.Value.ToString();
        PlayerHealth.text = health.Value.ToString();
        Spawn();
    }
    public void Spawn()
    {
        rm.enabled = false;
        Vector3 pos = Relay.Singleton.Spawns[(int)NetworkManager.Singleton.LocalClientId];
        transform.position = pos;
        StartCoroutine(SpawnWait());
    }

    IEnumerator SpawnWait()
    {
        yield return new WaitForSeconds(2f);
        rm.enabled = true;

    }
    [ServerRpc(RequireOwnership =false)]
    public void SpawnServerRpc()
    {

        health.Value = 10;
        alive.Value = true;
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
        //displayName.Value = playerData.Username;
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
        /*        Debug.Log(Relay.Singleton.clientData[0]);
                Debug.Log(Relay.Singleton.clientData[1]);*/
        //RelativeMovement rm = GetComponent<RelativeMovement>();
        SpawnServerRpc();


    }

    private void HandlingDeath()
    {
        // 
        Relay.Singleton.EndingGameServerRpc(NetworkManager.Singleton.LocalClientId);
    }
    private void HandleDisplayNameChanged(FixedString32Bytes oldDisplayName, FixedString32Bytes newDisplayName)
    {
        PlayerText.text = newDisplayName.ToString();
        //displayName.Value = newDisplayName.ToString();
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
