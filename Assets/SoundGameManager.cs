using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using Unity.Netcode.Components;
using System;

public class SoundGameManager : NetworkBehaviour
{
    [SerializeField] private AudioSource UIAudioSource;

    public SoundLibrary soundList;
    private void OnEnable()
    {
        //UIAudioSource.PlayOneShot(Resources.Load("sfx/" + "miss") as AudioClip);
    }
    [ClientRpc]
    public void PlayOnceClientRpc(string clip)
    {
    /*    UIAudioSource = gameObject.AddComponent<AudioSource>();
        UIAudioSource.clip = Resources.Load("sfx/" + clip) as AudioClip;
        UIAudioSource.Play();*/
        UIAudioSource.PlayOneShot(Resources.Load("sfx/" + clip) as AudioClip);
        Debug.LogError("Play");
    }
    [ServerRpc(RequireOwnership = false )]
    public void PlayOnceServerRpc(string clip)
    {
        PlayOnceClientRpc(clip);
    }

}
