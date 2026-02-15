using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private WarriorMinion _blueWarriorPrefab;
    [SerializeField] private WarriorMinion _redWarriorPrefab;
    [SerializeField] private MageMinion _blueMagePrefab;
    [SerializeField] private MageMinion _redMagePrefab;

    // Arrays of spawn points for each team
    [SerializeField] private Transform[] _blueSpawnPoints;
    [SerializeField] private Transform[] _redSpawnPoints;

    private readonly int _numOfMinionsInWave = 3;
    private readonly float _waveSpawnDelay = 10;
    private readonly float _minionSpawnDelay = 2;

    private void Awake()
    {
        RegisterMinionPool(_blueWarriorPrefab, Entity.Team.Blue);
        RegisterMinionPool(_blueMagePrefab, Entity.Team.Blue);
        RegisterMinionPool(_redWarriorPrefab, Entity.Team.Red);
        RegisterMinionPool(_redMagePrefab, Entity.Team.Red);
    }

    private void Start()
    {
        StartCoroutine(SpawnMinionWaves());
    }


    private void RegisterMinionPool(Minion prefab, Entity.Team team)
    {
        PoolFactory.Instance.SetPoolActions(
            prefab,
            onGet: (minion) =>
            {
                minion.ResetHp();
                minion.ClearTargets();
                //minion.ResetState();
                minion.EntityTeam = team;
                minion.gameObject.SetActive(true);
            },
            onRelease: (minion) =>
            {
                //minion.ClearTarget();
                minion.gameObject.SetActive(false);
            }
        );
    }


    // Coroutine to spawn waves of minions
    private IEnumerator SpawnMinionWaves()
    {
        while (true)
        {
            // Spawn warriors for each team at each spawn point
            for (int i = 0; i < _numOfMinionsInWave; i++)
            {
                SpawnMinionsAtPositions("Warrior", _blueSpawnPoints, "Blue");
                SpawnMinionsAtPositions("Warrior", _redSpawnPoints, "Red");
                yield return new WaitForSeconds(_minionSpawnDelay); //delay between each spawn
            }

            // Spawn mages for each team at each spawn point
            for (int i = 0; i < _numOfMinionsInWave; i++)
            {
                SpawnMinionsAtPositions("Mage", _blueSpawnPoints, "Blue");
                SpawnMinionsAtPositions("Mage", _redSpawnPoints, "Red");
                yield return new WaitForSeconds(_minionSpawnDelay); //delay between each spawn
            }

            // Wait 10 seconds before starting the next wave
            yield return new WaitForSeconds(_waveSpawnDelay);
        }
    }

    // Method to spawn minions at multiple positions for a given team
    private void SpawnMinionsAtPositions(string type, Transform[] spawnPoints, string team)
    {
        foreach (Transform spawnPoint in spawnPoints)
        {
            SpawnMinion(type, team, spawnPoint.position);
        }
    }

    // Method to spawn a minion at the specified position
    private void SpawnMinion(string type, string team, Vector3 spawnPosition)
    {
        Minion prefab = GetPrefab(type, team);
        if (prefab != null)
        {
            Minion minion = PoolFactory.Instance.GetObject(prefab);
            minion.transform.position = spawnPosition;

            // Set team and type
            minion.EntityTeam = team == "Blue" ? Entity.Team.Blue : Entity.Team.Red;
            minion.EntityType = Entity.Type.Minion;
            minion.gameObject.SetActive(true);
        }
        else
        {
            Debug.LogError($"Invalid type or team: {type}, {team}");
        }
    }

    // Determine which prefab to use based on type and team
    private Minion GetPrefab(string type, string team)
    {
        if (team == "Blue")
        {
            if (type == "Warrior") return _blueWarriorPrefab;
            if (type == "Mage") return _blueMagePrefab;
        }
        else if (team == "Red")
        {
            if (type == "Warrior") return _redWarriorPrefab;
            if (type == "Mage") return _redMagePrefab;
        }
        return null;
    }

    // Despawns the minion, returning it to the correct pool
    public void DespawnMinion(Minion minion)
    {
        PoolFactory.Instance.ReleaseObject(minion);
    }

/*    void Start()
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
        minion.transform.position = transform.position;
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
*/
}
