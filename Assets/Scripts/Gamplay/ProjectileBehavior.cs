using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ProjectileBehavior : NetworkBehaviour
{
    public float Speed = 20.0f;
    public float DestroyTime = 2f;
    public int Damage = 1;
    public int type = 0;
    public bool Stop = false;
    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(DestroyNetworkObject(DestroyTime));
    }
    private void OnEnable()
    {
        Relay.Singleton.GameStarted.OnValueChanged += GameStartedHandleChange;
    }

    private void OnDisable()
    {
        Relay.Singleton.GameStarted.OnValueChanged -= GameStartedHandleChange;
    }
    private void GameStartedHandleChange(bool previousValue, bool newValue)
    {
        DestroyObjectServerRpc();
    }
    // Update is called once per frame
    void Update()
    {
        if (Stop) return;
        transform.Translate(0, 0, Speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.LogWarning("ENTER");
        PlayerInGame player = other.GetComponent<PlayerInGame>();
        ShootBehavior playerShoot = other.GetComponent<ShootBehavior>();

        if (player != null)
        {
            if (Stop)
            {
                playerShoot.canShoot = true; 
                ManagersInGame.SoundManager.PlayOnceClientRpc(
     "reload"
     );
                DestroyObjectServerRpc();

            }
            else
            {
             ManagersInGame.SoundManager.PlayOnceClientRpc(
               "hit"
               );
                player.HurtServerRpc(Damage);
            }
        }
        else
        {
            ManagersInGame.SoundManager.PlayOnceClientRpc(
    "miss"
    );
            Stop = true;
        }
        if (!IsOwner && gameObject != null) return;
/*        DestroyObjectServerRpc();
        Destroy(gameObject);*/
    }
    IEnumerator DestroyNetworkObject(float timeToDestroy)
    {
        yield return new WaitForSeconds(timeToDestroy);
        DestroyObjectServerRpc();
        Destroy(gameObject);
    }

    [ServerRpc(RequireOwnership =false)]

    void DestroyObjectServerRpc()
    {
        gameObject.GetComponent<NetworkObject>().Despawn();

    }
}
