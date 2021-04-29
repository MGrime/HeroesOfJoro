using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuIdler : MonoBehaviour
{
    [SerializeField] private GameObject[] _playersToRotate;

    [SerializeField] private float _timeBetweenSwitch;

    private float _currentTime;
    private int _activeIndex;

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
}
