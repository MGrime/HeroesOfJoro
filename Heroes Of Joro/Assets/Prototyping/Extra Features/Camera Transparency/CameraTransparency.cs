using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Use a raycast between camera and player to temporarily set transparency
// Keep a list of modified objects and set back to normal if they are not in the currently colliding list
// This is a fairly expensive script but its not a trivial process. Therefore I run it on a coroutine to limit calls
public class CameraTransparency : MonoBehaviour
{
    #region Editor Fields

    [SerializeField] private GameObject _camera;
    [SerializeField] private GameObject _player;

    [SerializeField] private Material _transparencyMaterial;
    [SerializeField] private Material _normalMaterial;

    #endregion

    #region Private Data

    private float _cameraPlayerDistance;
    private List<MeshRenderer> _currentlyTransparentObjects;
    private List<MeshRenderer> _hitThisFrame;

    #endregion

    #region Functions

    private void Start()
    {
        _cameraPlayerDistance = Math.Abs((_camera.transform.position - _player.transform.position).magnitude);

        _currentlyTransparentObjects = new List<MeshRenderer>();
        _hitThisFrame = new List<MeshRenderer>();
    }

    private void Update()
    {
        RaycastForWalls();

    }

    private void RaycastForWalls()
    {
        // Draw a ray between player and camera
        Ray currentRay = new Ray(_camera.transform.position, _player.transform.position - _camera.transform.position);
        RaycastHit[] hits;

        hits = Physics.RaycastAll(currentRay,_cameraPlayerDistance);

        foreach (RaycastHit hit in hits)
        {
            // Try to get the mesh renderer
            MeshRenderer meshRenderer = hit.transform.gameObject.GetComponent<MeshRenderer>();
            if (meshRenderer)
            {
                // Set it to be a transparent material
                meshRenderer.material = _transparencyMaterial;

                // Add to the hit list if not already in
                if (!_currentlyTransparentObjects.Exists(x => x == meshRenderer))
                {
                    // Add to list
                    _currentlyTransparentObjects.Add(meshRenderer);
                }
            }

            // Always add to frame list
            _hitThisFrame.Add(meshRenderer);

        }

    }

    private void ChangeBackObjects()
    {
        // For each object in the _currentlyTransparentObject list
        // Check if it was a hit this frame
        // If not set back to a solid material and remove from the list
        foreach (MeshRenderer meshRenderer in _currentlyTransparentObjects)
        {
            // If it is does not exist then it was NOT hit this frame so set back and remove
            if (!_hitThisFrame.Exists(x => x == meshRenderer))
            {
                meshRenderer.material = _normalMaterial;

                _currentlyTransparentObjects.Remove(meshRenderer);
            }
        }
    }

    #endregion
}
