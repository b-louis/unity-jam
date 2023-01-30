using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    public void GoMenu()
    {
        Debug.Log("Menu");
        SceneManager.LoadScene("Menu");
    }

    public void GoGame()
    {
        SceneManager.LoadScene("Game");
        Relog();
    }

    public void Relog()
    {
        //int response = await Managers.Metafab.AuthPlayer(Managers.Player.Username, Managers.Player.Password);
        Managers.GameEvents.LoginEvent.Invoke();
    }
}
