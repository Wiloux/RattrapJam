using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Debris : MonoBehaviour
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

	public float strength;

	public List<GameObject> skins = new List<GameObject>();
	// Start is called before the first frame update
	void Start()
	{
		skins[Random.Range(0, skins.Count)].SetActive(true);


		pulledTarget = this.transform;
		targetBody = GetComponent<Rigidbody>();
		player = FindObjectOfType<PlayerController>();
		pullingObject = player.transform;
		StartCoroutine(JustSpawned());
	}

	IEnumerator Death()
	{
		Vector3 initscale = transform.lossyScale;
		Vector3 initPosition = transform.position;
		float elapsedTime = 0;
		float waitTime = 0.8f;

		while (elapsedTime < waitTime)
		{
			transform.localScale = Vector3.Lerp(initscale, Vector3.zero, (elapsedTime / waitTime));
			transform.position = Vector3.Lerp(initPosition, player.transform.position, (elapsedTime / waitTime));
			elapsedTime += Time.deltaTime;

			// Yield here
			yield return null;
		}
		player.UpdateScaleAndStrength(null, this);

		Destroy(gameObject);
	}


	bool justSpawned = true;
	IEnumerator JustSpawned()
	{
		yield return new WaitForSeconds(0.3f);
		justSpawned = false;
	}

	// Update is called once per frame
	void Update()
	{
		if (justSpawned)
			return;


		distanceToPlayer = Vector3.Distance(pulledTarget.position, pullingObject.position);
		if (distanceToPlayer < influenceRange * transform.localScale.magnitude)
		{
			StartCoroutine(Death());
		}

	}

	private void OnDrawGizmosSelected()
	{
		Gizmos.DrawWireSphere(transform.position, influenceRange * transform.localScale.magnitude);
	}
}
