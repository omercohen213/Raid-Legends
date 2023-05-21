using System.Collections;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject redMageMinionPrefab;
    [SerializeField] private GameObject blueMageMinionPrefab;
    [SerializeField] private Transform blueMinionsSpawner;
    [SerializeField] private Transform redMinionsSpawner;

    void Start()
    {
        StartCoroutine(SpawnMinions());
    }

    private IEnumerator SpawnMinions()
    {
        int spawnCount = 0;
        while (true)
        {
            if (spawnCount < 5)
            {
                Spawn(Entity.Team.Blue);
                Spawn(Entity.Team.Red);
                spawnCount++;
            }
            else
            {
                yield return new WaitForSeconds(10f); // Wait for 10 seconds before repeating the spawning
                spawnCount = 0;
            }

            yield return new WaitForSeconds(2f); // Wait for 2 seconds before spawning the next minion
        }
    }

    public void Spawn(Entity.Team team)
    {
        GameObject minion;
        GameObject minionPrefab = null;
        Transform spawnerTransform = null;

        if (team == Entity.Team.Blue)
        {
            minionPrefab = blueMageMinionPrefab;
            spawnerTransform = blueMinionsSpawner;
        }

        else if (team == Entity.Team.Red)
        {
            minionPrefab = redMageMinionPrefab;
            spawnerTransform = redMinionsSpawner;
        }

        minion = Instantiate(minionPrefab, spawnerTransform.transform.position, Quaternion.identity, spawnerTransform);
    }
}
