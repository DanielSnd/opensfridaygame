using UnityEngine;
using System.Collections;

public class EnemySpawner : MonoBehaviour {

	public Transform enemyPrefab;
    public float minSpawnTime = 0.3f;
    public float maxSpawnTime = 2f;

	// Use this for initialization
	void Start () {
		enemyPrefab.CreatePool ();
	    StartCoroutine(SpawnEnemies());
	}

    IEnumerator SpawnEnemies()
    {
        yield return new WaitForSeconds(1.5f);
        while (PlayerController.instance != null && !PlayerController.instance.healthCtrl.Dead)
        {
            enemyPrefab.Spawn(new Vector3(transform.position.x + Random.Range(-10, +10), transform.position.y, transform.position.z + Random.Range(-10, +10)));
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
        }
    }

}
