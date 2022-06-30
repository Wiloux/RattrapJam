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
	private float distanceToPull;
	private float distanceToPlayer;
	private Vector3 pullForce;
	public float playerStrengthPercentage;
	private Vector3 initialScale;
	public Transform killerTransform;
	public Material currentMat;
	public GameObject rot;

	public GameObject planetExplosionFX;

	public ParticleSystem suckParticles;
	private static ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1000];
	public float rotationSpeed = 50;
	private Vector3 rotationAxis = new Vector3(0, 0, 1);
	int count;
	public Transform darkerHole;
	public Enemy myEnemy;

	public float maxDistance;
	public float distanceToDarkerHole;

	private Spawner spawner;


	// Start is called before the first frame update
	void Start()
	{
		killerTransform = null;
		pulledTarget = this.transform;
		targetBody = GetComponent<Rigidbody>();
		player = FindObjectOfType<PlayerController>();
		myEnemy = GetComponent<Enemy>();
		pullingObject = player.transform;
		initialScale = transform.localScale;
		spawner = FindObjectOfType<Spawner>();
		darkerHole = FindObjectOfType<Blackhole>().transform;

		if (!isDebris && rot != null)
		{
			currentMat = rot.GetComponentInChildren<MeshRenderer>().material;
			currentMat.SetFloat("Health", myEnemy.strength / myEnemy.strength);
		}
		distanceToDarkerHole = Mathf.Infinity;
	}

	private float t = 0;
	// Update is called once per frame
	void Update()
	{


		if (myEnemy.strength <= 0 && !myEnemy.dead)
		{
			StartCoroutine(Death(true, player.transform));
		}

		suckParticlesFunc();

		distanceToPlayer = Vector3.Distance(pulledTarget.position, pullingObject.position);

		pullingObject = player.transform;
		distanceToPull = distanceToPlayer;


		//Destroy if too far
		if (distanceToPull > maxDistance * pullingObject.localScale.magnitude)
		{
			Destroy(gameObject);
			spawner.currentUnits--;
		}

		if (pullingObject == player.transform)
		{
			if (distanceToPull <= influenceRange * transform.localScale.magnitude && myEnemy.strength * 0.65f < player.strength)
			{
				transform.RotateAround(pullingObject.position, rotationAxis, rotationSpeed * Time.deltaTime);
				pullForce = (pullingObject.position - pulledTarget.position).normalized / distanceToPull * intensity;
				targetBody.AddForce(pullForce);

				if (!isDebris && currentMat != null)
					currentMat.SetFloat("Health", myEnemy.strength / myEnemy.strengthMax);

				if (transform.localScale.magnitude > pullingObject.localScale.magnitude) //If Im bigger than the player
				{
					myEnemy.strength -= (player.strength * playerStrengthPercentage) * Time.deltaTime;
					if (!suckParticles.isPlaying)
						suckParticles.Play();
				}
				else //If I'm smaller
				{
					if (suckParticles.isPlaying)
						suckParticles.Stop();

					Debug.Log("killed by grav pull");
					StartCoroutine(Death(false, player.transform));
				}

			}
			else if (distanceToPull > influenceRange)
			{
				if (suckParticles.isPlaying)
					suckParticles.Stop();
				//targetBody.velocity = Vector3.zero;
			}
		}
	}


	void suckParticlesFunc()
	{
		count = suckParticles.GetParticles(particles);

		for (int i = 0; i < count; i++)
		{
			ParticleSystem.Particle particle = particles[i];

			Vector3 v1 = suckParticles.transform.TransformPoint(particle.position);
			Vector3 v2 = player.transform.position;

			Vector3 tarPosi = (v2 - v1) * (particle.lifetime / particle.startLifetime);
			particle.position = suckParticles.transform.InverseTransformPoint(v2 - tarPosi);
			particles[i] = particle;
		}

		suckParticles.SetParticles(particles, count);
	}
	public IEnumerator Death(bool spawnDebris = false, Transform killerTransform = null)
	{
		//killerTransform = killer;
		myEnemy.dead = true;
		Vector3 initscale = transform.lossyScale;
		Vector3 initPosition = transform.position;
		float elapsedTime = 0;
		float waitTime = 0.8f;


		if (!spawnDebris || player.rampaging)
		{
			//No debris
			while (elapsedTime < waitTime)
			{
				transform.localScale = Vector3.Lerp(initscale, Vector3.zero, (elapsedTime / waitTime));
				transform.position = Vector3.Lerp(initPosition, killerTransform.transform.position, (elapsedTime / waitTime));
				elapsedTime += Time.deltaTime;

				// Yield here
				yield return null;
			}


			if (pullingObject == player.gameObject.transform)
			{
				player.UpdateScaleAndStrength(myEnemy);
			}

		}
		else
		{
			if (killerTransform.gameObject == player.gameObject)
			{
				CameraShake.Instance.ShakeCamera(2,0.5f);
				SoundManager.instance.PlayClip(1);

				//yield return new WaitForSeconds(0.3f);
				List<Debris> debris = new List<Debris>();
				int debrisAmount = Random.Range(2, 6);
				for (int i = 0; i < debrisAmount; i++)
				{
					Vector3 explosionDir = new Vector3((int)Random.Range(-1, 1),0, (int)Random.Range(-1, 1));
					Debris newenemy = Instantiate(Debris, transform.position, Quaternion.identity).GetComponent<Debris>();
					debris.Add(newenemy);
					newenemy.GetComponent<Rigidbody>().AddForce(Random.Range(7, 10) * explosionDir, ForceMode.Impulse);
					Destroy(newenemy.gameObject, 10f);
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
		}

		spawner.currentUnits--;
		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		//if (transform.localScale.magnitude < player.transform.localScale.magnitude)
		//{
		//	StartCoroutine(Death(other.transform));
		//}
	}

	private void OnDrawGizmosSelected()
	{
		if (player != null)
			Gizmos.DrawWireSphere(transform.position, maxDistance * player.transform.localScale.magnitude);

		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, influenceRange * transform.localScale.magnitude);
	}
}

