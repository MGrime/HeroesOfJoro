using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonValidator : MonoBehaviour
{
    public bool Valid = true;

    // This will trigger when this piece is placed on another component
    private void OnTriggerStay(Collider other)
    {
        // We need to delete this component
        // So set a bool for the dungeon class to use
         Valid = false;

         //Debug.Log("COLLIDING\n");
    }

    private void OnTriggerExit(Collider other)
    {
        Valid = true;

        //Debug.Log("EXIT COLLIDING\n");
    }

}
