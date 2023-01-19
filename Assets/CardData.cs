using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Unity.Collections;

public struct CardData : INetworkSerializable, IEquatable<CardData>
{

    public int DamageAmount;
    public List<int> EffectIndex;

    public CardData(int damageAmount, List<int> effectIndex)
    {
        DamageAmount = damageAmount;
        EffectIndex = effectIndex;
    }

    public bool Equals(CardData other)
    {
        return DamageAmount == other.DamageAmount &&
                EffectIndex == other.EffectIndex;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeValue(ref DamageAmount);

        int length = 0;
        int[] Array = EffectIndex.ToArray();
        if (!serializer.IsReader)
        {
            length = Array.Length;
        }

        serializer.SerializeValue(ref length);

        for (int n = 0; n < length; ++n)
        {
            serializer.SerializeValue(ref Array[n]);
        }
    }
}