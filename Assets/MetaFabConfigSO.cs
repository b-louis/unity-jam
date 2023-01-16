using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MetafabSdk;

[CreateAssetMenu(fileName = "MetaFabConfig", menuName = "ScriptableObjects/MetaFabConfigScriptableObject", order = 1)]
public class MetaFabConfigSO : ScriptableObject
{
    public string gameId;

    public string email;
    public string xAuthorization;

    public string secretKey;
    public string publicKey;
    public string wallet;
    public string walletId;
    public string fundingWallet;
    public string fundingWalletId;

    public string currencyId;
    public string contractId;

    public string shopId;
    public string collectionId;

    private void OnEnable()
    {
        MetafabSdk.Config.Email = email;
        MetafabSdk.Config.Password = xAuthorization;
        MetafabSdk.Config.PublishedKey = publicKey;

    }
}
