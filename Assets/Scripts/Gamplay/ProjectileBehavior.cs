using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBehavior : NetworkBehaviour
{
    public float Speed = 5.0f;
    public float DestroyTime = 2f;
    public float Damage = 5.0f;
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

    IEnumerator DestroyNetworkObject(float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);
        DestroyObjectServerRpc();
        Destroy(gameObject);
    }

    [ServerRpc]

    void DestroyObjectServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();

    }
}
