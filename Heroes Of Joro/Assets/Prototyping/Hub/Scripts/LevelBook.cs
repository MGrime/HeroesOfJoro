using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBook : MonoBehaviour
{
    [SerializeField] private GameObject _overlayCanvas;

    private PlayerManager _playerManager;

    private bool _isActive = false;

    public void Start()
    {
        _playerManager = FindObjectOfType<PlayerManager>();
    }
    public void Update()
    {
        // If the user presses button 
        if (Input.GetKeyDown(KeyCode.F) && !_isActive)
        {
            float distance = Vector3.Distance(_playerManager.ActivePlayer.transform.position, transform.position);

            // Within 5 units
            if (distance <= 5.0f)
            {
                Debug.Log("Activated Book!");
                // Open the overlay
                _overlayCanvas.SetActive(true);

                // Force disable movement
                _playerManager.ActivePlayer.GetComponent<ThirdPersonMovementScript>().enabled = false;
                _playerManager.SetCameraRotation(false);
                // Deactive book interaction
                _isActive = true;
            }
        }
    }

    public void CloseBook()
    {
        // Close the overlay
        _overlayCanvas.SetActive(false);

        // Force enable movement
        _playerManager.ActivePlayer.GetComponent<ThirdPersonMovementScript>().enabled = true;
        _playerManager.SetCameraRotation(true);


        // activate book interaction
        _isActive = false;
    }
}
