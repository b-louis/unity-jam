using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventsManager : MonoBehaviour
{
    public UnityEvent LoginEvent;
    public UnityEvent<float> BalanceChangedEvent;

}
