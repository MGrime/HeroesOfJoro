using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBase : MonoBehaviour
{
    public enum PlayerType
    {
        Mage,
        Warrior
    }

    private PlayerType _type;

    public PlayerType Type
    {
        get => _type;
        set => _type = value;
    }


}
