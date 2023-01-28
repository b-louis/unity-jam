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
    public bool canShoot = true;
    // Start is called before the first frame update
    // Update is called once per frame
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
    [ServerRpc]
    private void ShootServerRpc()
    {
        _projectile = Instantiate(projectilePrefab) as GameObject;
        _projectile.transform.position = transform.TransformPoint(Vector3.forward * 1.5f);
        _projectile.transform.rotation = transform.rotation;
        _projectile.GetComponent<NetworkObject>().Spawn();
    }
}
