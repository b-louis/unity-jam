using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBehavior : NetworkBehaviour
{
    public float Speed = 5.0f;
    public float DestroyTime = 2f;
    public int Damage = 5;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DestroyNetworkObject(DestroyTime));
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(0, 0, Speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("ENTER");
        PlayerInGame player = other.GetComponent<PlayerInGame>();
        if (player != null)
        {
            Debug.Log("AIE!");

            player.HurtServerRpc(Damage);
        }
        if (!IsOwner && gameObject != null) return;
        DestroyObjectServerRpc();
        Destroy(gameObject);
    }
    IEnumerator DestroyNetworkObject(float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);
        DestroyObjectServerRpc();
        Destroy(gameObject);
    }

    [ServerRpc]

    void DestroyObjectServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();

    }
}
