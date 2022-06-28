using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravitationnalPull : MonoBehaviour
{
	[SerializeField] private Transform pulledTarget; public Transform pullingObject;
	private Rigidbody targetBody;
	[SerializeField] private float influenceRange;
	[SerializeField] private float intensity;
	private float distanceToPlayer;
	private Vector3 pullForce;
	// Start is called before the first frame update
	void Start()
	{
		pulledTarget = this.transform;
		targetBody = GetComponent<Rigidbody>();
		pullingObject = FindObjectOfType<PlayerController>().transform;
	}

	// Update is called once per frame
	void Update()
	{
		distanceToPlayer = Vector3.Distance(pulledTarget.position, pullingObject.position);
		if (distanceToPlayer < influenceRange)
		{
			pullForce = (pullingObject.position - pulledTarget.position).normalized / distanceToPlayer * intensity;
			targetBody.AddForce(pullForce);
		}
		else if (distanceToPlayer > influenceRange + influenceRange / 2)
		{
			targetBody.velocity = Vector3.zero;
		}
	}
}
