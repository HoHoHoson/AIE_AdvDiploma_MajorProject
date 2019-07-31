using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnTest : MonoBehaviour
{
    
    [Tooltip("How many enemies need to be spawned.")]
    public int enemies_to_spawn;

    // spawn width(x) and height(z) for the area that the enemies will spawn
    [Tooltip("Enter Spawn Area. \n width = x, height = z")]
    public int spawn_width, spawn_height;

    // spawn points for enemy and spawn count
    private int spawn_pos_x, spawn_pos_z, spawn_count;

    // Enemy types
    [Tooltip("Add in default enemy of type")]
    public GameObject enemy_normal, enemy_explosion, enemy_boss;

    // Enemy spawn points
    public GameObject spawn_point1, spawn_point2, spawn_point3, spawn_point4;
    
    // Wave counts
    public int wave = 1, wave_next;

    private int spawn_at_object;
    private int spawn_object_type, boss_spawn_object_type;
    private GameObject spawn_location;
    private GameObject enemy_type;
    private int num_of_normal, num_of_explode, num_of_boss;

    public int enemy_explosion_spawnrate = 5;
    public int boss_spawnrate = 20;
    public bool next_wave_now = false;
    private bool spawned_all = false;
    int current_wave;

    public GameObject[] enemy_list;

    private void Start()
    {
        Waves();
    }

    void Update()
    {
        if (next_wave_now == true)
        {
            wave++;
            StopAllCoroutines();
            foreach(var enemy in enemy_list)
            {
                if (enemy != null && spawned_all == true)
                {
                    Destroy(enemy);
                }
            }

            spawn_count = 0;
            wave_next = wave + 1;
            next_wave_now = false;
        }
        if (wave > current_wave)
        {
            Waves();
            
        }
    }

    public int GetCurrentWave()
    {
        return wave;
    }

    public void Waves()
    {
        current_wave = wave;
        if (wave < 2)
        {
            spawned_all = false;
            StartCoroutine(SpawnNormalEnemies(enemies_to_spawn));
            
        }
        else if (wave < 4)
        {
            spawned_all = false;
            StartCoroutine(SpawnEnemies(enemies_to_spawn));
            
        }
        else
        {
            spawned_all = false;
            StartCoroutine(SpawnEnemiesWithBoss(enemies_to_spawn));
           
        }
    }

    
    IEnumerator SpawnNormalEnemies(int NumOfEnemies)
    {
        enemy_list = new GameObject[NumOfEnemies];
        while (spawn_count < NumOfEnemies)
        {
            spawn_at_object = Random.Range(0, 4);

            switch (spawn_at_object)
            {
                case 0:
                    spawn_location = spawn_point1;
                    break;
                case 1:
                    spawn_location = spawn_point2;
                    break;
                case 2:
                    spawn_location = spawn_point3;
                    break;
                case 3:
                    spawn_location = spawn_point4;
                    break;
                case 4:
                    spawn_location = spawn_point1;
                    break;
                default:
                    break;
            }

            spawn_pos_x = Random.Range(-spawn_width / 2, spawn_width / 2);
            spawn_pos_z = Random.Range(-spawn_height / 2, spawn_height / 2);

            enemy_list[spawn_count] = Instantiate(enemy_normal, new Vector3(spawn_location.transform.position.x + spawn_pos_x, 
                spawn_location.transform.position.y,spawn_location.transform.position.z + spawn_pos_z), Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
            num_of_normal++;
            spawn_count++;
        }
        spawned_all = true;
    }

    IEnumerator SpawnEnemies(int NumOfEnemies)
    {
        enemy_list = new GameObject[NumOfEnemies];
        while (spawn_count < NumOfEnemies)
        {
            spawn_at_object = Random.Range(0, 4);

            switch (spawn_at_object)
            {
                case 0:
                    spawn_location = spawn_point1;
                    break;
                case 1:
                    spawn_location = spawn_point2;
                    break;
                case 2:
                    spawn_location = spawn_point3;
                    break;
                case 3:
                    spawn_location = spawn_point4;
                    break;
                case 4:
                    spawn_location = spawn_point1;
                    break;
                default:
                    break;
            }
            spawn_object_type = Random.Range(0, enemy_explosion_spawnrate);

            if (spawn_object_type == enemy_explosion_spawnrate - 1)
            {
                enemy_type = enemy_explosion;
                num_of_explode++;
            }
            else
            {
                enemy_type = enemy_normal;
                num_of_normal++;
            }

            spawn_pos_x = Random.Range(-spawn_width / 2, spawn_width / 2);
            spawn_pos_z = Random.Range(-spawn_height / 2, spawn_height / 2);

            enemy_list[spawn_count] = Instantiate(enemy_type, new Vector3(spawn_location.transform.position.x + spawn_pos_x,
                spawn_location.transform.position.y, spawn_location.transform.position.z + spawn_pos_z), Quaternion.identity);

            yield return new WaitForSeconds(0.1f);
            spawn_count++;
        }
        spawned_all = true;
    }

    IEnumerator SpawnEnemiesWithBoss(int NumOfEnemies)
    {
        enemy_list = new GameObject[NumOfEnemies];
        while (spawn_count < NumOfEnemies)
        {
            spawn_at_object = Random.Range(0, 4);

            switch (spawn_at_object)
            {
                case 0:
                    spawn_location = spawn_point1;
                    break;
                case 1:
                    spawn_location = spawn_point2;
                    break;
                case 2:
                    spawn_location = spawn_point3;
                    break;
                case 3:
                    spawn_location = spawn_point4;
                    break;
                case 4:
                    spawn_location = spawn_point1;
                    break;
                default:
                    break;
            }

            spawn_object_type = Random.Range(0, enemy_explosion_spawnrate);
            boss_spawn_object_type = Random.Range(0, boss_spawnrate);

            if (spawn_object_type == enemy_explosion_spawnrate - 1)
            {
                enemy_type = enemy_explosion;
                num_of_explode++;
            }
            else if(boss_spawn_object_type == boss_spawnrate - 1 && spawn_count < NumOfEnemies - 10)
            {
                enemy_type = enemy_boss;
                num_of_boss++;
                spawn_count = spawn_count + 9;
            }
            else
            {
                enemy_type = enemy_normal;
                num_of_normal++;
            }

            spawn_pos_x = Random.Range(-spawn_width / 2, spawn_width / 2);
            spawn_pos_z = Random.Range(-spawn_height / 2, spawn_height / 2);

            enemy_list[spawn_count] = Instantiate(enemy_type, new Vector3(spawn_location.transform.position.x + spawn_pos_x,
                spawn_location.transform.position.y, spawn_location.transform.position.z + spawn_pos_z), Quaternion.identity);
            
            yield return new WaitForSeconds(0.1f);
            spawn_count++;
        }
        spawned_all = true;
    }
}
