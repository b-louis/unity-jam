using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameSceneManager : MonoBehaviour
{
    public void GoMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void GoGame()
    {
        SceneManager.LoadScene("Game");

    }
}
