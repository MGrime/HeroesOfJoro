using TMPro;
using UnityEngine;

public class MummyCounter : MonoBehaviour
{
    TMP_Text _text;
    int _mummiesAlive;

    void Awake() => _text = GetComponent<TMP_Text>();

    private void Start()
    {
        Mummy.Spawned += Mummy_Spawned;
        Mummy.Died += Mummy_Died;
    }

    private void Mummy_Died()
    {
        _mummiesAlive--;
        _text.SetText(_mummiesAlive.ToString());
    }

    private void Mummy_Spawned()
    {
        _mummiesAlive++;
        _text.SetText(_mummiesAlive.ToString());
    }
}