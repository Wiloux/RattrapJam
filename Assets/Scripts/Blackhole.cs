using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
	public Transform playerTransform;
	public PlayerController playerScript;
	public Transform chasedTransform;
	public float speed;
	public float force = 2;
	public float destroyDistance = 5;
	public Vector3 targetScale;
	public float lerpTime = 1;
	public float growDivider = 5;
	public float attractionRange;
	public Spawner spawner;
	public GameObject eye;
	public bool sucked = false;

	public bool isAwake;
	

	public LayerMask everythingbutPlayer;
	// Start is called before the first frame update
	void Start()
	{
		targetScale = this.transform.localScale;
		playerTransform = FindObjectOfType<PlayerController>().transform;
		playerScript = FindObjectOfType<PlayerController>();
		chasedTransform = playerTransform;
		spawner = FindObjectOfType<Spawner>();
	}

	// Update is called once per frame
	void Update()
	{

		if (!isAwake)
			return;
		if(sucked == false) {
		targetScale = Vector3.Lerp(playerScript.minSize + playerScript.minSize * 0.1f,
			playerScript.maxSize + playerScript.maxSize * 0.1f,
			playerScript.t);
		}

		Collider[] hitColliders = Physics.OverlapSphere(transform.position, transform.lossyScale.x + attractionRange, everythingbutPlayer);


		if (hitColliders.Length != 0)
		{
			float minDistance = 5000000;
			Transform bestTarget = null;

			for (int i = 0; i < hitColliders.Length; i++)
			{
				if (hitColliders[i].transform.lossyScale.x > transform.lossyScale.x)
					break;

				if (hitColliders[i].gameObject.GetComponent<GravitationnalPull>() != null)
				{
					
					if (hitColliders[i].gameObject.GetComponent<GravitationnalPull>().myEnemy.isDead)
					{		
						break;
					}
				}

				if (Vector3.Distance(transform.position, hitColliders[i].transform.position) < minDistance)
				{
					minDistance = Vector3.Distance(transform.position, hitColliders[i].transform.position);
					bestTarget = hitColliders[i].transform;
				}
			}


			if (bestTarget != null)
				chasedTransform = bestTarget.transform;
			else
				chasedTransform = playerTransform;
		}
		else
		{
			chasedTransform = playerTransform;
		}

		if(playerScript.rampaging == false)
        {
		speed = playerScript.moveSpeed * 0.5f;

        }

		//if (Vector3.Distance(playerTransform.position, this.transform.position) <= attractionRange)
		//{
		//    playerTransform.position = Vector3.MoveTowards(playerTransform.position, this.transform.position, 0.01f * Time.deltaTime);
		//}
		if (this.transform.localScale.magnitude < targetScale.magnitude && sucked == false)
		{
			this.transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpTime * Time.deltaTime);
			//attractionRange = this.transform.localScale.x / 2 * 17;
		}
		if (chasedTransform.gameObject != null)
		{
			transform.position = Vector3.MoveTowards(transform.position, chasedTransform.position, speed * Time.deltaTime);

			//else
			//{
			//	chasedTransform = playerTransform;
			//}

			if (Vector3.Distance(chasedTransform.position, this.transform.position) < destroyDistance)
			{
				//if (chasedTransform.gameObject.CompareTag("Consumable") == false)
				//{
				//	targetScale += chasedTransform.localScale / growDivider;
				//}
				if (chasedTransform != playerTransform)
				{

					if (chasedTransform != null&& chasedTransform.gameObject.GetComponent<GravitationnalPull>() != null)
					{
						StartCoroutine(chasedTransform.gameObject.GetComponent<GravitationnalPull>().Death(false, transform));

						chasedTransform = null;
					}
				}
				else
				{
					if (playerTransform.gameObject.GetComponent<PlayerController>().rampaging == false)
					{
						playerTransform.gameObject.GetComponent<PlayerController>().Death();
					}
					else
					{
						EndGame();
					}
				}
			}

			if(chasedTransform != null)
			eye.transform.localEulerAngles = new Vector4(0, (transform.position.y - chasedTransform.position.y), (transform.position.x - chasedTransform.position.x), (transform.position.y - chasedTransform.position.y)).normalized * 30;

		}


	}

	void EndGame()
	{
		//gameObject.SetActive(false);
	}

	private void OnDrawGizmosSelected()
	{

		Gizmos.DrawWireSphere(transform.position, transform.lossyScale.x + attractionRange);
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, transform.lossyScale.x + destroyDistance);
	}
}
//    void OnTriggerEnter(Collider other)
//    {

//        if (other.GetComponent<GravitationnalPull>() != null && transform.localScale.magnitude > other.transform.localScale.magnitude)
//        {
//            Debug.Log("xD");
//            chasedTransform = other.transform;
//            //other.GetComponent<GravitationnalPull>().pullingObject = this.transform;
//            other.GetComponent<GravitationnalPull>().darkerHole = this.transform;
//        }


//    }

//    void OnTriggerExit(Collider other)
//    {

//        if (other.GetComponent<GravitationnalPull>() != null && transform.localScale.magnitude > other.transform.localScale.magnitude)
//        {
//            chasedTransform = playerTransform;
//            //other.GetComponent<GravitationnalPull>().pullingObject = playerTransform;
//        }


//    }
//}
