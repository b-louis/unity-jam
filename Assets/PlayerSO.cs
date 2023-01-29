using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObjects/PlayerScriptableObject", order = 1)]
public class PlayerSO : ScriptableObject
{
    public string Username;
    public string AccessToken;
    public string loadout;
    public string WalletId;
    public string WalletAdress;
    public float Balance; 
    public int weapon;
    public int shades;
    public int hat;
    public string Playerid;

    public string Password { get; set; }
}
