using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HubPortal : MonoBehaviour
{
    public string _mainGameScene;
    public AudioSource _teleportSound;

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
        SceneManager.LoadScene(_mainGameScene);
    }
}
