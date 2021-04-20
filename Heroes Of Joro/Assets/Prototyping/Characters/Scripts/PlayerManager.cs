using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //Simple reference to allow tracking the player
    #region Singleton

    public static PlayerManager _instance;

    #endregion
    public PlayerBase[] PlayerTrackers;

    public PlayerBase ActivePlayer;

    public CinemachineFreeLook CameraFreeLook;

    public Camera MinimapCamera;

    private bool CameraIsActive = true;

    #region Private Data

    private GameObject _manaBar;  // Find it with find
    private GameObject _activeSpell;
    [SerializeField] LayerMask _aimLayerMask;

    #endregion

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
        MinimapCamera.transform.SetParent(ActivePlayer.transform);

        _manaBar = GameObject.Find("Mana Bar");
        _activeSpell = GameObject.Find("Active Spell");

        CameraFreeLook.Follow = ActivePlayer.GetComponentInChildren<Animator>().transform;
        CameraFreeLook.LookAt = ActivePlayer.GetComponentInChildren<Animator>().transform;
   
    }

    void Update()
    {
        if (CameraIsActive)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (ActivePlayer != PlayerTrackers[0] && PlayerTrackers[0].Health > 0)
                {
                    SwitchCharacter(ref PlayerTrackers[0]);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                if (ActivePlayer != PlayerTrackers[1] && PlayerTrackers[1].Health > 0)
                {
                    SwitchCharacter(ref PlayerTrackers[1]);
                }
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3))
            {
                if (ActivePlayer != PlayerTrackers[2] && PlayerTrackers[2].Health > 0)
                {
                    SwitchCharacter(ref PlayerTrackers[2]);
                }
            }

            // Update this pos to be same as character
            gameObject.transform.position = ActivePlayer.transform.position;
        }
            AimTowardMouse();
    }

    public void SetCameraRotation(bool state)
    {
        CameraIsActive = state;
        CameraFreeLook.enabled = state;
    }

    void SwitchCharacter(ref PlayerBase newPlayer)
    {
        // Find type of player to modify UI
        if (_manaBar)
        {
            Debug.Log(newPlayer.Type);
            if (newPlayer.Type == PlayerBase.PlayerType.Mage)
            {
                _manaBar.SetActive(true);
                _activeSpell.SetActive(true);

                Debug.Log("Activated");
            }
            else
            {
                _manaBar.SetActive(false);
                _activeSpell.SetActive(false);
                Debug.Log("Deactivated");
            }
        }

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

        MinimapCamera.transform.SetParent(newPlayer.transform);

    }

    void DisablePlayer(PlayerBase player)
    {
        player.enabled = false;
        player.GetComponentInChildren<ThirdPersonMovementScript>().enabled = false;
        //player.gameObject.GetComponent<AnimationScript>().enabled = false;
        player.gameObject.SetActive(false);
    }

    // Called by player when a death happens
    public void PlayerDied()
    {
        // Check players to find first one still alive
        bool foundPlayer = false;
        for (int i = 0; i < PlayerTrackers.Length; ++i)
        {
            if (PlayerTrackers[i].Health > 0.0f)
            {
                foundPlayer = true;
                SwitchCharacter(ref PlayerTrackers[i]);
                break;
            }
        }

        // If no players alive
        if (!foundPlayer)
        {
            // End game
            Debug.Log("haha game done brr");
        }
    }


    void EnablePlayer(PlayerBase player)
    {
        player.enabled = true;
        player.GetComponentInChildren<ThirdPersonMovementScript>().enabled = true;
        //player.gameObject.GetComponent<AnimationScript>().enabled = true;
        player.gameObject.SetActive(true);
    }

    void AimTowardMouse()
    {
        Ray ray = GetComponentInChildren<Camera>().ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hitInfo, Mathf.Infinity, _aimLayerMask))
        {
            var _direction = hitInfo.point - ActivePlayer.transform.position;
            _direction.y = 0.0f;
            _direction.Normalize();
            ActivePlayer.transform.forward = _direction;
        }
    }
}
