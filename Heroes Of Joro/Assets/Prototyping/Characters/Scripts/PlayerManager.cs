﻿using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Simple reference to allow tracking the player
    #region Singleton

    public static PlayerManager _instance;

    #endregion
    public GameObject[] PlayerTrackers;

    public GameObject ActivePlayer;

    public CinemachineFreeLook CameraFreeLook;
   
    void Start()
    {
        _instance = this;
        // TODO: THIS IS BECAUSE OF SOMETHING I DID IN PROC GEN. OBVIOUSLY NEEDS TO TWEAK FOR OTHER PLAYER TYPES. ADD A BASE CLASS FOR PLAYER TYPES

        foreach (var player in PlayerTrackers)
        {
            DisablePlayer(player);
        }

        //To make it easier instead of tracking the player track the camera which has access to the player being controlled
        EnablePlayer(PlayerTrackers[0]);

        ActivePlayer = PlayerTrackers[0];

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (ActivePlayer != PlayerTrackers[0])
            {
                SwitchCharacter(ref PlayerTrackers[0]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (ActivePlayer != PlayerTrackers[1])
            {
                SwitchCharacter(ref PlayerTrackers[1]);
            }
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            if (ActivePlayer != PlayerTrackers[2])
            {
                SwitchCharacter(ref PlayerTrackers[2]);

            }
        }
    }

    void SwitchCharacter(ref GameObject newPlayer)
    {
        // Move to active player
        newPlayer.transform
            .SetPositionAndRotation(ActivePlayer.transform.position, ActivePlayer.transform.rotation);

        // Disable active player
        DisablePlayer(ActivePlayer);

        // Enable and switch
        EnablePlayer(newPlayer);
        ActivePlayer = newPlayer;

        // Get the transform of the object that the animator is attached to
        CameraFreeLook.Follow = newPlayer.GetComponentInChildren<Animator>().transform;
        CameraFreeLook.LookAt = newPlayer.GetComponentInChildren<Animator>().transform;
    }

    void DisablePlayer(GameObject player)
    {
        player.GetComponent<Mage>().enabled = false;
        player.GetComponentInChildren<ThirdPersonMovementScript>().enabled = false;
        player.GetComponent<AnimationScript>().enabled = false;
        player.SetActive(false);
    }


    void EnablePlayer(GameObject player)
    {
        player.GetComponent<Mage>().enabled = true;
        player.GetComponentInChildren<ThirdPersonMovementScript>().enabled = true;
        player.GetComponent<AnimationScript>().enabled = true;
        player.SetActive(true);
    }
}
