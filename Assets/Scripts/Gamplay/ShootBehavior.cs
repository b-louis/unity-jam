using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ShootBehavior : NetworkBehaviour
{
    private Vector3 _mousePos;

    [SerializeField] private GameObject projectilePrefab;
    
    private GameObject _projectile;

    public float RateOfFire;
    public bool canShoot = false;
    // Start is called before the first frame update
    // Update is called once per frame
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
        StartCoroutine(WaitToShoot(newValue));
    }
    IEnumerator WaitToShoot(bool value)
    {
        yield return new WaitForSeconds(2);
        canShoot = value;

    }
    void Update()
    {
        if (!IsOwner) return;
        if (Input.GetMouseButtonDown(0) && canShoot)
        {
            canShoot = false;
            _mousePos = GetComponent<RelativeMovement>().mousePoint;
            Ray ray = new Ray(transform.position, _mousePos- transform.position);
            RaycastHit hit;
            /*		RaycastHit[] hits;
                    hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);*/
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                ShootServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership =false)]
    private void ShootServerRpc()
    {
        ManagersInGame.SoundManager.PlayOnceClientRpc(
            "spear"
           
            ) ;
        ShootClientRpc();
        //_projectile.GetComponent<NetworkObject>().Spawn();
    }
    [ClientRpc]
    private void ShootClientRpc()
    {
        _projectile = Instantiate(projectilePrefab) as GameObject;
        _projectile.transform.position = transform.TransformPoint(Vector3.forward * 2.5f);
        _projectile.transform.rotation = transform.rotation;
    }
}
