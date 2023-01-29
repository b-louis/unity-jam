using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MetafabManager))]
[RequireComponent(typeof(GameEventsManager))]
[RequireComponent(typeof(GameSceneManager))]

public class Managers : MonoBehaviour
{
    public static MetafabManager Metafab;
    public static GameEventsManager GameEvents;
    public static GameSceneManager GameSceneManager;
    [SerializeField]
    private PlayerSO PlayerInstance;
    public static PlayerSO Player;
    // Start is called before the first frame update
    private void Awake()
    {
        GameEvents = GetComponent<GameEventsManager>();
        GameSceneManager = GetComponent<GameSceneManager>();
        DontDestroyOnLoad(gameObject);
    }
    void Start()
    {
        Player = PlayerInstance;
        Metafab = GetComponent<MetafabManager>();

    }

}
