using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationnalPull : MonoBehaviour
{

	public bool isDebris;
	public GameObject Debris;
	[SerializeField] private Transform pulledTarget;
	[SerializeField] public Transform pullingObject;
	[SerializeField] public PlayerController player;
	private Rigidbody targetBody;
	[SerializeField] public float influenceRange;
	[SerializeField] private float intensity;
	private float distanceToPlayer;
	private Vector3 pullForce;
	public float playerStrengthPercentage;
	private Vector3 initialScale;
	private Vector3 zAxis = new Vector3(0, 0, 1);
	public int rotationSpeed;
	public Material currentMat;
	public GameObject rot;

	public GameObject planetExplosionFX;

	private Enemy myEnemy;

	// Start is called before the first frame update
	void Start()
	{
		pulledTarget = this.transform;
		targetBody = GetComponent<Rigidbody>();
		player = FindObjectOfType<PlayerController>();
		myEnemy = GetComponent<Enemy>();
		pullingObject = player.transform;
		initialScale = transform.localScale;
		influenceRange *= initialScale.magnitude;
		if (!isDebris && rot != null)
		{
			currentMat = rot.GetComponentInChildren<MeshRenderer>().material;
			currentMat.SetFloat("Health", myEnemy.strength / myEnemy.strength);
		}

	}

	private float t = 0;
	// Update is called once per frame
	void Update()
	{


		if (myEnemy.strength <= 0 && !myEnemy.dead)
		{
			StartCoroutine(Death(true));
		}


		distanceToPlayer = Vector3.Distance(pulledTarget.position, pullingObject.position);
		if (distanceToPlayer < influenceRange)
		{
			transform.RotateAround(pullingObject.position, zAxis, rotationSpeed * Time.deltaTime);
			pullForce = (pullingObject.position - pulledTarget.position).normalized / distanceToPlayer * intensity;
			targetBody.AddForce(pullForce);

			if (!isDebris && currentMat != null)
				currentMat.SetFloat("Health", myEnemy.strength / myEnemy.strengthMax);


			if (transform.localScale.magnitude > player.transform.localScale.magnitude) //If Im bigger than the player
			{
				myEnemy.strength -= (player.strength * playerStrengthPercentage) * Time.deltaTime;
			}
			else //If I'm smaller
			{
				//if (transform.localScale.magnitude > 0.01f)  //If I'm bigger than nothing
				//{
				//	transform.localScale -= Vector3.one * (player.strength * playerStrengthPercentage) * Time.deltaTime;
				//}
				//else
				//{
				StartCoroutine(Death());
				//}
			}

		}
		else if (distanceToPlayer > influenceRange + influenceRange / 2)
		{
			//targetBody.velocity = Vector3.zero;
		}
	}



	IEnumerator Death(bool spawnDebris = false)
	{
		myEnemy.dead = true;
		Vector3 initscale = transform.lossyScale;
		Vector3 initPosition = transform.position;
		float elapsedTime = 0;
		float waitTime = 0.8f;

		if (!spawnDebris)
		{

			while (elapsedTime < waitTime)
			{
				transform.localScale = Vector3.Lerp(initscale, Vector3.zero, (elapsedTime / waitTime));
				transform.position = Vector3.Lerp(initPosition, player.transform.position, (elapsedTime / waitTime));
				elapsedTime += Time.deltaTime;

				// Yield here
				yield return null;
			}

			player.UpdateScaleAndStrength(myEnemy);
		}
		else
		{
			yield return new WaitForSeconds(1);
			List<Debris> debris = new List<Debris>();
			int debrisAmount = Random.Range(2, 6);
			for (int i = 0; i < debrisAmount; i++)
			{
				Vector2 explosionDir = new Vector2(Random.Range(-1, 1), Random.Range(-1,1));
				Debris newenemy = Instantiate(Debris, transform.position, Quaternion.identity).GetComponent<Debris>();
				debris.Add(newenemy);
				newenemy.GetComponent<Rigidbody>().AddForce(Random.Range(15, 20) * explosionDir, ForceMode.Impulse);
			}

			float debrisStrength = myEnemy.strengthMax / debrisAmount;

			for (int i = 0; i < debrisAmount; i++)
			{
				debris[i].strength = debrisStrength;		
				debris[i].transform.localScale = initscale;
			}

		GameObject exFX = Instantiate(planetExplosionFX, transform.position, Quaternion.identity);
		exFX.transform.localScale = initscale;
		Destroy(exFX, 2);

		}

		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (transform.localScale.magnitude < player.transform.localScale.magnitude)
		{
			StartCoroutine(Death());
		}
	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, influenceRange * transform.localScale.magnitude);
	}
}

