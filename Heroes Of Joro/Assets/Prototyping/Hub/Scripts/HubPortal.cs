using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubPortal : MonoBehaviour
{
    // Configure where this portal sends us to
    // First variable sets scene. Second variable signals in scene load which dungeon to activate
    [SerializeField] private string _mainGameScene = "";
    [SerializeField] private int _targetDungeon = 0;

    // Optional sound effect for teleportation
    [SerializeField] private AudioSource _teleportSound = null;

    // Marks if this portal exits the game
    [SerializeField] private bool _isExit = false;

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
        {
            return;
        }

        if (_teleportSound)
        {
            _teleportSound.ignoreListenerVolume = true;
            _teleportSound.Play();
        }
        if (_isExit)
        {
            Application.Quit();
            Debug.Log("Sent player application quit command!");
        }
        else
        {
            PlayerPrefs.SetInt("TargetDungeon",_targetDungeon);
            PlayerPrefs.Save();

            SceneManager.LoadScene(_mainGameScene);
        }
        
    }
}
