using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Simple reference to allow tracking the player
    #region Singleton

    public static PlayerManager _instance;

    #endregion
    public GameObject PlayerTracker;
   
    void Start()
    {
        _instance = this;
        // TODO: THIS IS BECAUSE OF SOMETHING I DID IN PROC GEN. OBVIOUSLY NEEDS TO TWEAK FOR OTHER PLAYER TYPES
        //To make it easier instead of tracking the player track the camera which has access to the player being controlled
        PlayerTracker.GetComponent<Mage>().enabled = true;
        PlayerTracker.GetComponentInChildren<ThirdPersonMovementScript>().enabled = true;
        
    }
}
