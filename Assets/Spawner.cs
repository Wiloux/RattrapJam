using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	// Start is called before the first frame update
	public int MAXUNIT = 500;
	public Vector2 maxUnitsMinMax; 
	public int currentUnits;

	public Vector2 size;
	public Vector2 sizeMinMax; 
	public float miniDistanceFromPlayer;

	public GameObject player;
	public PlayerController playerScript;
	public float playerStrength = 3;
	public float strengthMax = 5;
	public GameObject enemy;
	public Blackhole blackHole;


	public float timerDur;
	private float timer;

	public Vector3 maxSize;
	public Vector3 minSize;

	public float miniSpaceSpawn;

	public float t;

	void Start()
	{
		timer = timerDur;
		playerScript = player.GetComponent<PlayerController>();
		blackHole = FindObjectOfType<Blackhole>();
	}

	// Update is called once per frame
	void Update()
	{
		MAXUNIT = (int)Mathf.Lerp(maxUnitsMinMax.x, maxUnitsMinMax.y, playerScript.t);
		size = Vector2.one * (int)Mathf.Lerp(sizeMinMax.x, sizeMinMax.y, playerScript.t);

		if(playerScript.strength >= 1.5 && blackHole.isAwake != true)
		{
			blackHole.isAwake = true;
			SoundManager.instance.PlayClip(3);
		}


		if (timer >= 0)
		{
			timer -= Time.deltaTime;
		}
		else
		{
			spawnUnit(playerStrength);
			timer = timerDur;
		}
		playerStrength = playerScript.strength;
	}

	public void spawnUnit(float currentPlayerStrength)
	{


		if (MAXUNIT <= currentUnits)
			return;


		Vector3 final = player.transform.position + (new Vector3(Random.Range(-size.x / 2, size.x / 2),
			Random.Range(-size.y / 2, size.y / 2)));

		float randomRange = Random.Range(currentPlayerStrength - 1.5f, currentPlayerStrength + 1.5f);

		if (randomRange < 1)
			randomRange = 1;

		if (randomRange > strengthMax)
			randomRange = strengthMax;

		t = (randomRange - 1) / strengthMax;

		Vector3 scale = Vector3.Lerp(minSize, maxSize, t);
		Collider[] hitColliders = Physics.OverlapSphere(final, scale.x * miniSpaceSpawn);
		int maxAttempt = 0;

		while ((hitColliders.Length != 0 || Vector3.Distance(player.transform.position, final) < playerScript.miniMapCam.orthographicSize ) && maxAttempt != 50)
		{
			final = player.transform.position + (new Vector3(Random.Range(-size.x / 2, size.x / 2),
			Random.Range(-size.y / 2, size.y / 2)));

			hitColliders = Physics.OverlapSphere(final, scale.x * miniSpaceSpawn);
			maxAttempt++;
		}

		if (hitColliders.Length != 0)
			return;

		if (maxAttempt >= 50)
			return;

		GameObject newEnemy = Instantiate(enemy, final, Quaternion.identity);

		currentUnits++;
		newEnemy.transform.localScale = scale;
		newEnemy.GetComponent<Enemy>().strength = randomRange;
	}

	private void OnDrawGizmos()
	{
		if (playerScript != null && player != null)
		{

			Gizmos.color = Color.yellow;
			Gizmos.DrawWireCube(player.transform.position, new Vector3(size.x, size.y));
			Gizmos.color = Color.blue;
			Gizmos.DrawWireSphere(player.transform.position,  playerScript.miniMapCam.orthographicSize);


		}
	}
}
