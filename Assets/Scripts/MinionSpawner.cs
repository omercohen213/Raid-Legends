using System.Collections;
using UnityEngine;

public class MinionSpawner : MonoBehaviour
{
    [SerializeField] private GameObject minionPrefab;
    [SerializeField] private Transform blueMinions;
    [SerializeField] private Transform redMinions;
    [SerializeField] private Transform blueMinionsSpawner;
    [SerializeField] private Transform redMinionsSpawner;
    private Minion minion;
    private readonly float minionSize = 0.5f;

    // Start is called before the first frame update
    private void Awake()
    {
        if (minionPrefab == null)
            Debug.Log("Missing minion prefab");
    }

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
        if (team == Entity.Team.Blue)
        {
            minion = Instantiate(minionPrefab, blueMinionsSpawner.transform.position, Quaternion.identity, blueMinions);
            minion.transform.localScale = new Vector3(minionSize, minionSize);
            minion.GetComponent<Minion>().EntityTeam = Entity.Team.Blue;
            minion.GetComponent<SpriteRenderer>().color = Color.blue;
        }
        else if (team == Entity.Team.Red)
        {
            minion = Instantiate(minionPrefab, redMinionsSpawner.transform.position, Quaternion.identity, redMinions);
            minion.transform.localScale = new Vector3(-minionSize, minionSize);
            minion.GetComponent<Minion>().EntityTeam = Entity.Team.Red;
            minion.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }
}
