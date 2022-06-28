using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationnalPull : MonoBehaviour
{
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

	public Material currentMat;
	public GameObject rot;

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

		currentMat = rot.GetComponentInChildren<MeshRenderer>().material;
		currentMat.SetFloat("Health", myEnemy.strength / myEnemy.strength);
	}

	private float t = 0;
	// Update is called once per frame
	void Update()
	{


		if (myEnemy.strength <= 0 && !myEnemy.dead)
		{
			StartCoroutine(Death());
		}


		distanceToPlayer = Vector3.Distance(pulledTarget.position, pullingObject.position);
		if (distanceToPlayer < influenceRange)
		{
			pullForce = (pullingObject.position - pulledTarget.position).normalized / distanceToPlayer * intensity;
			targetBody.AddForce(pullForce);

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
			targetBody.velocity = Vector3.zero;
		}
	}



	IEnumerator Death(bool spawnDebris = false)
	{
		myEnemy.dead = true;
		Vector3 initscale = transform.localScale;
		float elapsedTime = 0;
		float waitTime = 2f;
		if (spawnDebris)
			waitTime /= 1.5f;

		while (elapsedTime < waitTime)
		{
			transform.localScale = Vector3.Lerp(initscale, Vector3.zero, (elapsedTime / waitTime));
			elapsedTime += Time.deltaTime;

			// Yield here
			yield return null;
		}
		if(!spawnDebris)
		player.UpdateScaleAndStrength(myEnemy);


		Destroy(gameObject);
	}

	private void OnTriggerEnter(Collider other)
	{
		if (transform.localScale.magnitude < player.transform.localScale.magnitude)
		{
			StartCoroutine(Death());
		}
	}
}
