using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlatformButton : MonoBehaviour
{
    [SerializeField] Material _onMaterial;
    [SerializeField] Material _offMaterial;
    [SerializeField] UnityEvent _onEnter;
    [SerializeField] UnityEvent _onExit;
    [SerializeField] float _exitDelay = 5f;

    Renderer _renderer;
    void Awake() => _renderer = GetComponent<Renderer>();

    void OnEnable() => _onExit.Invoke();

    void OnTriggerEnter(Collider other)
    {
        _renderer.material = _onMaterial;
        _onEnter.Invoke();
    }

    void OnTriggerExit(Collider other)
    {
        _renderer.material = _offMaterial;
        StartCoroutine(DelayExitEvent());
       
    }

    IEnumerator DelayExitEvent()
    {
        yield return new WaitForSeconds(_exitDelay);
        _onExit.Invoke();
    }
}