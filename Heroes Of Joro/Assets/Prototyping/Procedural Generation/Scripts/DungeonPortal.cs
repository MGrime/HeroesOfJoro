using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonPortal : MonoBehaviour
{
    public Dungeon _dungeon;
    public int _targetFloor;


    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        _dungeon.SwitchFloor(_targetFloor);
    }

}
