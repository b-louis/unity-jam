using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainMenuUiController : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI PlayerBalance;
    private void Awake()
    {
        Managers.GameEvents.BalanceChangedEvent.AddListener(OnBalanceChange);

    }
    private void OnDestoy()
    {
        Managers.GameEvents.BalanceChangedEvent.RemoveListener(OnBalanceChange);

    }
    private void OnBalanceChange(float balance)
    {
        Debug.Log("changed balance");
        PlayerBalance.text = "Balance : "+ balance;
        //Managers.Metafab.UseShopOffer(1.ToString());
        //Managers.Metafab.TransfertItem(1, "0xC30a7Ce684eFf6c76e3745CeeA464aDD3b711375", 1);
    }

    public void OnBalanceClick(float balance)
    {
        Debug.Log("balance click");
        PlayerBalance.text = "Balance : " + balance;
        //Managers.Metafab.UseShopOffer(1.ToString());
        //Managers.Metafab.TransfertItem(1, "0xC30a7Ce684eFf6c76e3745CeeA464aDD3b711375", 1);
    }

}
