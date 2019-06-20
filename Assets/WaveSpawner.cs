using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WaveSpawner : MonoBehaviour
{

    public enum SpawnState {Spawning, Waiting, Counting};

    public Transform spawnPoint;

    [System.Serializable]
    public class Wave
    {
        public string name;
        public Transform enemyPrefab;
        public int count;
        public float SpawnRate;
    }

    public Wave[] waves;
    private int nextWave = 0;

    public float timeBetweenWaves = 5f;
    public float waveCountdown = 2f;

    public Text waveCountdownText;

    private float enemySearchCountdown = 1f;

    private SpawnState state = SpawnState.Counting;

    void Start ()
    {
        waveCountdown = timeBetweenWaves;
    }

    void Update ()
    {

        if (state == SpawnState.Waiting)
        {
            // Check if all enemys dead
            if (!EnemyIsAlive())
            {
                //Begin new round
                WaveCompleted();
            }
            else
            {
                return;
            }
        }

        if (waveCountdown <= 0f)
        {
            if (state != SpawnState.Spawning)
            {
                // Start spawning wave
                StartCoroutine(SpawnWave(waves[nextWave])); //Start Coroutine needed for IEnumate
            }
        }
        else
        {
            waveCountdown -= Time.deltaTime;

            waveCountdownText.text = Mathf.Floor(waveCountdown + 1).ToString();
        }
    }

    void WaveCompleted ()
    {
        state = SpawnState.Counting;
        waveCountdown = timeBetweenWaves;

        //Game state complete...edit to make harder ect
        if (nextWave + 1 > waves.Length -1)
        {
            nextWave = 0;
        }
        else
        {
            nextWave++;
        }
    }

    bool EnemyIsAlive ()
    {
        enemySearchCountdown -= Time.deltaTime;

        if (enemySearchCountdown <= 0f)
        {
            enemySearchCountdown = 1f;
            if (GameObject.FindGameObjectWithTag("Enemy") == null)
            {
                return false;
            }
        }
        return true;
    }

    IEnumerator SpawnWave (Wave _wave) // allows a break between enemy spawning (This is a Coroutine)
    {
        state = SpawnState.Spawning;
        //Spawn
        for (int i = 0; i < _wave.count; i++)
        {
            SpawnEnemy(_wave.enemyPrefab);
            yield return new WaitForSeconds(1f / _wave.SpawnRate);
        }

        state = SpawnState.Waiting;


        yield break;
    }

    void SpawnEnemy (Transform _enemy)
    {
        //Spawn Enemy
        Instantiate(_enemy, spawnPoint.position, spawnPoint.rotation);
    }
}
