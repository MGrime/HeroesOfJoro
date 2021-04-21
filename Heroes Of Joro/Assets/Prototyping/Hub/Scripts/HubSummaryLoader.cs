using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HubSummaryLoader : MonoBehaviour
{
    [SerializeField] private Canvas _wonCanvas;
    [SerializeField] private Canvas _lostCanvas;

    // Start is called before the first frame update
    public void Start()
    {
        // Check if player prefs has the key
        if (PlayerPrefs.HasKey("ReturnReason"))
        {
            int returnValue = PlayerPrefs.GetInt("ReturnReason");

            if (returnValue != -1)
            {
                GameManager.DungeonEndReason returnReason =
                    (GameManager.DungeonEndReason) Mathf.Clamp(returnValue, 0, 1);

                if (returnReason == GameManager.DungeonEndReason.Lost)
                {
                    _lostCanvas.enabled = true;
                }
                else if (returnReason == GameManager.DungeonEndReason.Won)
                {
                    _wonCanvas.enabled = true;
                }

            }

        }
    }

    public void CloseCanvas()
    {
        _lostCanvas.enabled = false;
        _wonCanvas.enabled = false;
    }

}
