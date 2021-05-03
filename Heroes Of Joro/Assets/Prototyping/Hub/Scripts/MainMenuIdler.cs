using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Simple script to rotate player on the main menu
public class MainMenuIdler : MonoBehaviour
{
    #region EDITOR FIELDS

    [SerializeField] private GameObject[] _playersToRotate = null;
    [SerializeField] private float _timeBetweenSwitch = 0.0f;

    #endregion

    #region VARIABLES

    private float _currentTime;
    private int _activeIndex;

    #endregion

    #region FUNCTIONS

    // Start is called before the first frame update
    private void Start()
    {
        _currentTime = 0.0f;
        _activeIndex = 0;

        foreach (GameObject player in _playersToRotate)
        {
            player.SetActive(false);
        }

        _playersToRotate[_activeIndex].SetActive(true);
    }

    // Update is called once per frame
    private void Update()
    {
        _currentTime += Time.deltaTime;

        if (_currentTime > _timeBetweenSwitch)
        {
            int newPlayer = _activeIndex + 1;

            if (newPlayer == _playersToRotate.Length)
            {
                newPlayer = 0;
            }

            _playersToRotate[_activeIndex].SetActive(false);

            _playersToRotate[newPlayer].SetActive(true);

            _activeIndex = newPlayer;

            _currentTime = 0.0f;

        }
    }

    #endregion
}
