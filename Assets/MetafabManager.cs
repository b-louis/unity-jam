using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using MetafabSdk;
using IngameDebugConsole;
using System;
using System.Threading.Tasks;

public class MetafabManager : MonoBehaviour
{
    [SerializeField]
    public MetaFabConfigSO MetaFabConfig;

	private void Awake()
	{
		Managers.GameEvents.LoginEvent.AddListener(
		   async () => { await GetPlayerCBalance(); }
		   );

	}
	private void OnDestoy()
	{
		Managers.GameEvents.LoginEvent.RemoveListener(
		   async () => { await GetPlayerCBalance(); }
		   );
	}
	async UniTask AuthGame()
    {
		Debug.Log($"Games.AuthGame...");
		var response = await Metafab.GamesApi.AuthGame(MetaFabConfig.email, MetaFabConfig.xAuthorization, default);
		Debug.Log($"AuthenticateGame {response}");
		Config.PublishedKey = response.publishedKey;
		Metafab.PublishedKey = Config.PublishedKey;
		Metafab.SecretKey = response.secretKey;
		Metafab.Password = Config.Password;
	}
    async UniTaskVoid Start()
    {
		await AuthGame();
    }
	public async UniTask<int> AuthPlayer(string username,string password)
    {
        try
        {
			var response = await Metafab.PlayersApi.AuthPlayer(username, password);
			Debug.Log($"Created player: {response}");
			Managers.Player.Username = username;
			Managers.Player.Password = password;
			Managers.Player.AccessToken = response.accessToken;
			Managers.Player.WalletId = response.walletId;
			Managers.Player.WalletAdress = response.wallet.address;
			return 0;

        }catch(Exception ex)
        {
			Debug.LogError(ex);
			return 1;
        }
		// Envoi d'un message vers l'UI et les autres managers pour passer � l'etape suivante si tout ok
		// Sinon renvoi l'erreur sur l'Ui et demande a l'user de recommencer
	}
	public async UniTask<int> CreatePlayer(string username, string password)
	{
		try
		{
			var response = await Metafab.PlayersApi.CreatePlayer(new CreatePlayerRequest(username, password));
			Debug.Log($"Created player: {response}");
			Managers.Player.Username = username;
			Managers.Player.AccessToken = response.accessToken;
			return 0;

		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
			Debug.LogError(ex.Message);

			return 1;
		}
		// Envoi d'un message vers l'UI et les autres managers pour passer � l'etape suivante si tout ok
		// Sinon renvoi l'erreur sur l'Ui et demande a l'user de recommencer
	}
	/*
	 
	 ---------------- 
	 
	 */
	public async UniTask<int> GetCurrensyId(string username, string password)
	{
		// We get currensyId/ adress, collection Id /adress, 
		try
		{
			var response = await Metafab.CurrenciesApi.GetCurrencyBalance("", Managers.Player.WalletAdress, Managers.Player.WalletId);
			Debug.Log($"Created player: {response}");
			Managers.Player.Username = username;
			Managers.Player.AccessToken = username;
			return 0;

		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
			Debug.LogError(ex.Message);

			return 1;
		}
		// Envoi d'un message vers l'UI et les autres managers pour passer � l'etape suivante si tout ok
		// Sinon renvoi l'erreur sur l'Ui et demande a l'user de recommencer
	}
	public async UniTask GetPlayerCBalance()
	{
		try
		{
			float response = await Metafab.CurrenciesApi.GetCurrencyBalance(Managers.Metafab.MetaFabConfig.currencyId, Managers.Player.WalletAdress,Managers.Player.WalletId);
			Debug.Log($"Player: {response}");
			Managers.Player.Balance = response;
			Managers.GameEvents.BalanceChangedEvent.Invoke(response);

		}
		catch (Exception ex)
		{
			Debug.LogError(ex);
			Debug.LogError(ex.Message);

		}
		// Envoi d'un message vers l'UI et les autres managers pour passer � l'etape suivante si tout ok
		// Sinon renvoi l'erreur sur l'Ui et demande a l'user de recommencer
	}
	// buy lootbox

	/*
	 
	 ---------------- 
	 
	 */
	// execute shop offer
	public async UniTask UseShopOffer(float offerId)
    {
		Metafab.SecretKey = Managers.Player.AccessToken;
		Metafab.Password = Managers.Player.Password;
		var response = await Metafab.ShopsApi.UseShopOffer(MetaFabConfig.shopId, offerId.ToString() );
		Debug.Log(response);
		Metafab.SecretKey = "";
		Metafab.Password = "";
	}
	// execute shop offers
	public async UniTask GetShopOffers()
	{
		var offers = await Metafab.ShopsApi.GetShopOffers(MetaFabConfig.shopId);
		//var currencyOffer = offers[offers.Count - 2];
		var itemOffer = offers[offers.Count - 1];
	}
	public async UniTask TransfertCurrensy(string address,float amout)
	{
		var transfer = await Metafab.CurrenciesApi.TransferCurrency(MetaFabConfig.currencyId, new TransferCurrencyRequest(address, Managers.Player.WalletId,amout,1));
		//var currencyOffer = offers[offers.Count - 2];
	}
	public async UniTask TransfertItem(float collectionItemId, string address,int quantity = 1)
	{
		Metafab.SecretKey = Managers.Player.AccessToken;
		Metafab.Password = Managers.Player.Password;
		var transfer = await Metafab.ItemsApi.TransferCollectionItem(
			MetaFabConfig.collectionId, 
			collectionItemId, 
			new TransferCollectionItemRequest(address,new List<string>{  }, quantity)
			);
		Metafab.SecretKey = "";
		Metafab.Password = "";
	}
	public async UniTask<Dictionary<string,float>> GetPlayerItemCollection()
    {
		var itemBalances = await Metafab.ItemsApi.GetCollectionItemBalances(MetaFabConfig.collectionId, Managers.Player.WalletAdress, Managers.Player.WalletId);
		return itemBalances;
	}
	private void testEvent()
    {
		Debug.Log("Loginez");
    }
	public void test2Event()
	{
		Debug.Log("Loginez");
	}

}
