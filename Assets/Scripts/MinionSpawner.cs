using System.Collections;
using UnityEngine;
using UnityEngine.Pool;
using static Entity;

public class MinionSpawner : MonoBehaviour
{

    [SerializeField] private GameObject redMageMinionPrefab;
    [SerializeField] private GameObject blueMageMinionPrefab;
    [SerializeField] private Transform blueMinionsSpawner;
    [SerializeField] private Transform redMinionsSpawner;

    private readonly int numOfMinionsInWave = 5;
    private readonly float waveSpawnTime = 10;
    private readonly float minionSpawnTime = 2;

    ObjectPool<Minion> blueMinionsPool;
    ObjectPool<Minion> redMinionsPool;

    void Start()
    {
        blueMinionsPool = new ObjectPool<Minion>(CreateBlueMinion, OnMinionGet, OnMinionRelease, DestroyMinion);
        redMinionsPool = new ObjectPool<Minion>(CreateRedMinion, OnMinionGet, OnMinionRelease, DestroyMinion);
        StartCoroutine(SpawnMinions());
    }

    private IEnumerator SpawnMinions()
    {
        int spawnCount = 0;
        while (true)
        {
            if (spawnCount < numOfMinionsInWave)
            {
                Spawn();
                spawnCount++;
            }
            else
            {
                yield return new WaitForSeconds(waveSpawnTime); // Wait for 10 seconds before repeating the spawning
                spawnCount = 0;
            }

            yield return new WaitForSeconds(minionSpawnTime); // Wait for 2 seconds before spawning the next minion
        }
    }

    public void Spawn()
    {
        Minion blueMinion = blueMinionsPool.Get();
        Minion redMinion = redMinionsPool.Get();      

        blueMinion.transform.SetPositionAndRotation(blueMinionsSpawner.position, Quaternion.identity);
        redMinion.transform.SetPositionAndRotation(redMinionsSpawner.position, Quaternion.identity);
        blueMinion.transform.parent = blueMinionsSpawner;
        redMinion.transform.parent = redMinionsSpawner;
    }

    // Instantiate a new blue minion
    private Minion CreateBlueMinion()
    {      
        GameObject minionGo = Instantiate(blueMageMinionPrefab);
        Minion minion = minionGo.GetComponent<Minion>();
        minion.SetPool(blueMinionsPool);
        minionGo.SetActive(false);
        return minion;
    }

    // Instantiate a new red minion
    private Minion CreateRedMinion()
    {
        GameObject minionGo = Instantiate(redMageMinionPrefab);
        Minion minion = minionGo.GetComponent<Minion>();
        minion.SetPool(redMinionsPool);
        minionGo.SetActive(false);
        return minion;
    }

    // Enable the minion when retrieved from the pool
    private void OnMinionGet(Minion minion)
    {
        minion.gameObject.SetActive(true);
        minion.Hp = minion.MaxHp;
    }

    // Disable the minion when released back to the pool
    private void OnMinionRelease(Minion minion)
    {
        minion.gameObject.SetActive(false);
    }

    private void DestroyMinion(Minion minion)
    {
        Debug.Log("destroy");
        Destroy(minion.gameObject);
    }
}
