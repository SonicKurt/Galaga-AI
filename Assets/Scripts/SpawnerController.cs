using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerController : MonoBehaviour
{
    public int gridX;
    public int gridZ;
    public float gapSize;

    public GameObject stringerObject;
    public GameObject goeiObject;
    public GameObject bossGalagaObject;

    private Vector3[,] grid;

    private void Awake()
    {
        
    }

    // Start is called before the first frame update
    void Start()
    {
        grid = new Vector3[gridX, gridZ];
    }

    private void Update()
    {
        if (GameManager.Instance.state == GameState.LoadEnemies)
        {
            SpawnAliens();
            GameManager.Instance.UpdateGameState(GameState.EnemiesFall);
        }
    }

    void SpawnAliens()
    {
        float startingPosX = gapSize * -4;
        float xPos = startingPosX * gapSize;

        EnemyType enemy = EnemyType.Goei;
        LoadEnemyState enemyState = LoadEnemyState.Phase1;

        // Create the grid
        for (int i = 0; i < gridX; i++)
        {
            startingPosX = gapSize * -5;
            for (int j = 0; j < gridZ; j++)
            {
                Vector3 spawnPos = new Vector3(startingPosX, 0, i * gapSize) + transform.localPosition;

                grid[i, j] = spawnPos;

                /*
                switch (enemy)
                {
                    case EnemyType.Goei:
                        SpawnGoei(spawnPos);
                        break;

                    case EnemyType.Stringer:
                        SpawnStringer(spawnPos);
                        break;

                    case EnemyType.BossGalaga:
                        SpawnBossGalaga(spawnPos);
                        break;
                }
                */
                startingPosX += gapSize;
            }
        }

        // Perform the phases to load in the enemies correct.
        
        while (enemyState != LoadEnemyState.Done)
        {
            switch (enemyState)
            {
                case LoadEnemyState.Phase1:
                    for (int i = 0; i < 4; i++)
                    {
                        if (i < 2)
                        {
                            SpawnStringer(grid[i, 4]);
                            SpawnStringer(grid[i, 5]);
                        } else
                        {
                            SpawnGoei(grid[i, 4]);
                            SpawnGoei(grid[i, 5]);
                        }
                    }
                    enemyState = LoadEnemyState.Phase2;
                    break;
                case LoadEnemyState.Phase2:
                    for (int i = 4; i > 1; i--) {
                        if (i == 4)
                        {
                            for (int j = 3; j < 7; j++)
                            {
                                SpawnBossGalaga(grid[i, j]);
                            }
                        } else
                        {
                            SpawnGoei(grid[i, 3]);
                            SpawnGoei(grid[i, 6]);
                        }
                    }

                    enemyState = LoadEnemyState.Phase3;
                    break;

                case LoadEnemyState.Phase3:
                    for (int i = 3; i > 1; i--)
                    {
                        SpawnGoei(grid[i, 1]);
                        SpawnGoei(grid[i, 2]);
                        SpawnGoei(grid[i, 7]);
                        SpawnGoei(grid[i, 8]);
                    }

                    enemyState = LoadEnemyState.Phase4;
                    break;
                case LoadEnemyState.Phase4:
                    for (int i = 1; i > -1; i--)
                    {
                        SpawnStringer(grid[i, 2]);
                        SpawnStringer(grid[i, 3]);
                        SpawnStringer(grid[i, 6]);
                        SpawnStringer(grid[i, 7]);
                    }

                    enemyState = LoadEnemyState.Phase5;
                    break;
                case LoadEnemyState.Phase5:
                    for (int i = 1; i > -1; i--)
                    {
                        SpawnStringer(grid[i, 0]);
                        SpawnStringer(grid[i, 1]);
                        SpawnStringer(grid[i, 8]);
                        SpawnStringer(grid[i, 9]);
                    }

                    enemyState = LoadEnemyState.Done;
                    break;
            }
        }
    }

    void SpawnGoei(Vector3 spawnPos)
    {
        GameObject goei = Instantiate(goeiObject, spawnPos, Quaternion.identity);
        goei.transform.SetParent(transform);
    }

    void SpawnStringer(Vector3 spawnPos)
    {
        GameObject stringer = Instantiate(stringerObject, spawnPos, Quaternion.identity);
        stringer.transform.SetParent(transform);
    }

    void SpawnBossGalaga(Vector3 spawnPos)
    {
        GameObject bossGalaga = Instantiate(bossGalagaObject, spawnPos, Quaternion.identity);
        bossGalaga.transform.SetParent(transform);
    }
}
