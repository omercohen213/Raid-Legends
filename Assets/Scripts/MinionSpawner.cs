using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using static Entity;

public class MinionSpawner : MonoBehaviour
{

    [SerializeField] private GameObject _blueMageMinionPrefab;
    [SerializeField] private GameObject _redMageMinionPrefab;
    [SerializeField] private Transform _blueMinionsSpawner;
    [SerializeField] private Transform _redMinionsSpawner;

    private readonly int _numOfMinionsInWave = 5;
    private readonly float _waveSpawnTime = 10;
    private readonly float _minionSpawnTime = 2;

    private ObjectPool<Minion> _blueMinionsPool;
    private ObjectPool<Minion> _redMinionsPool;

    void Start()
    {
        _blueMinionsPool = new ObjectPool<Minion>(CreateBlueMinion, OnMinionGet, OnMinionRelease, DestroyMinion);
        _redMinionsPool = new ObjectPool<Minion>(CreateRedMinion, OnMinionGet, OnMinionRelease, DestroyMinion);
        StartCoroutine(SpawnMinions());
    }

    private IEnumerator SpawnMinions()
    {
        int spawnCount = 0;
        while (true)
        {
            if (spawnCount < _numOfMinionsInWave)
            {
                Spawn();
                spawnCount++;
            }
            else
            {
                yield return new WaitForSeconds(_waveSpawnTime); // Wait for 10 seconds before repeating the spawning
                spawnCount = 0;
            }

            yield return new WaitForSeconds(_minionSpawnTime); // Wait for 2 seconds before spawning the next minion
        }
    }

    public void Spawn()
    {
        Minion blueMinion = _blueMinionsPool.Get();
        Minion redMinion = _redMinionsPool.Get();      

        blueMinion.transform.SetPositionAndRotation(_blueMinionsSpawner.position, Quaternion.identity);
        redMinion.transform.SetPositionAndRotation(_redMinionsSpawner.position, Quaternion.identity);
        blueMinion.transform.parent = _blueMinionsSpawner;
        redMinion.transform.parent = _redMinionsSpawner;
    }

    // Instantiate a new blue minion
    private Minion CreateBlueMinion()
    {      
        GameObject minionGo = Instantiate(_blueMageMinionPrefab);
        Minion minion = minionGo.GetComponent<Minion>();
        minion.SetPool(_blueMinionsPool);
        minionGo.SetActive(false);
        return minion;
    }

    // Instantiate a new red minion
    private Minion CreateRedMinion()
    {
        GameObject minionGo = Instantiate(_redMageMinionPrefab);
        Minion minion = minionGo.GetComponent<Minion>();
        minion.SetPool(_redMinionsPool);
        minionGo.SetActive(false);
        return minion;
    }

    // Enable the minion when retrieved from the pool
    private void OnMinionGet(Minion minion)
    {
        minion.ResetHp();
        minion.gameObject.SetActive(true);        
    }

    // Disable the minion when released back to the pool
    private void OnMinionRelease(Minion minion)
    {
        minion.gameObject.SetActive(false);
    }

    private void DestroyMinion(Minion minion)
    {
        Destroy(minion.gameObject);
    }
}
