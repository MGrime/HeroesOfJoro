using System.Collections;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] float _delay = 10f;
    [SerializeField] Mummy _prefab;

    float _nextSpawnTime;
    float _timesSpawned;

    void Update()
    {
        if (ReadyToSpawn())
            StartCoroutine(Spawn());
    }

    bool ReadyToSpawn() => Time.time >= _nextSpawnTime;

    IEnumerator Spawn()
    {
        float finalDelay = _delay - _timesSpawned;
        _nextSpawnTime = Time.time + finalDelay;

        var instance = Instantiate(_prefab, transform.position, transform.rotation);
        GetComponent<Animator>().SetBool("Open", true);
        yield return new WaitForSeconds(2.5f);
        instance.StartWalking();
        yield return new WaitForSeconds(2.5f);
        GetComponent<Animator>().SetBool("Open", false);
        
        _timesSpawned++;
    }

}
