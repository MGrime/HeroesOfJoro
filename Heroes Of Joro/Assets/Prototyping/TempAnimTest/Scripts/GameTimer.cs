using TMPro;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    TMP_Text _text;

    void Awake() => _text = GetComponent<TMP_Text>();

    void Update()
    {
        int seconds = (int)Time.timeSinceLevelLoad;
        _text.SetText(seconds.ToString());
    }
}