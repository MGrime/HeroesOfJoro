using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubPortal : MonoBehaviour
{
    public string _mainGameScene;
    public AudioSource _teleportSound;

    public bool _isExit;

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
            SceneManager.LoadScene(_mainGameScene);
        }
        
    }
}
