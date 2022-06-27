using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	// Start is called before the first frame update
	public const int MAXUNIT = 500;
	private int currentUnits;

	public Vector2 size;
	public float miniDistanceFromPlayer;

	public GameObject player;
	private float playerStrength = 3;
	public GameObject enemy;

	public float timerDur;
	private float timer;

	void Start()
	{
		timer = timerDur;
	}

	// Update is called once per frame
	void Update()
	{
		if(timer >= 0)
		{
			timer -= Time.deltaTime;
		}
		else
		{
			spawnUnit(playerStrength);
			timer = timerDur;
		}
	}

	public void spawnUnit(float currentPlayerStrength)
	{
		if (MAXUNIT <= currentUnits)
			return;

		currentUnits++;

		Vector3 final = player.transform.position + new Vector3(Random.Range(-size.x / 2, size.x /2),
			0,
			Random.Range(-size.y / 2, size.y / 2));

		while(Vector3.Distance(player.transform.position, final) < miniDistanceFromPlayer)
		{
			final= player.transform.position + new Vector3(Random.Range(-size.x / 2, size.x / 2),
				0,
				Random.Range(-size.y / 2, size.y / 2));
		}


		float randomRange = Random.Range(currentPlayerStrength - 3, currentPlayerStrength + 3);
		if (randomRange < 1)
		{
			randomRange = 1;
		}

		GameObject newEnemy = Instantiate(enemy, final, Quaternion.identity);
		newEnemy.GetComponent<Enemy>().strength = randomRange;
	}

	private void OnDrawGizmos()
	{
		Gizmos.DrawWireCube(player.transform.position, new Vector3(size.x, 0, size.y));
		Gizmos.color = Color.blue;
		Gizmos.DrawWireSphere(player.transform.position, miniDistanceFromPlayer);
	}
}
