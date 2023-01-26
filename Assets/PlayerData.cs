using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Netcode.Components;
public struct PlayerData : INetworkSerializable
{
    public string Username;
    public string AccessToken;
    public string loadout;

    public PlayerData(string a)
    {
        Username = a;
        AccessToken = a;
        loadout = a;

    }
    // INetworkSerializable
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref Username);
        serializer.SerializeValue(ref AccessToken);
    }
}