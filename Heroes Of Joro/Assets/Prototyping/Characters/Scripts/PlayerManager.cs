using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Simple reference to allow tracking the player
    #region Singleton

    public static PlayerManager _instance;

    private void Awake()
    {
        _instance = this;
    }
    #endregion
    public GameObject _playerTracker;
}
