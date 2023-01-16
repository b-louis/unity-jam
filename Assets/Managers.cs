using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MetafabManager))]
[RequireComponent(typeof(GameEventsManager))]

public class Managers : MonoBehaviour
{
    public static MetafabManager Metafab;
    public static GameEventsManager GameEvents;
    [SerializeField]
    private PlayerSO PlayerInstance;
    public static PlayerSO Player;
    // Start is called before the first frame update
    private void Awake()
    {
        GameEvents = GetComponent<GameEventsManager>();
    }
    void Start()
    {
        Player = PlayerInstance;
        Metafab = GetComponent<MetafabManager>();

    }

}
