using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
	[SerializeField] private float moveSpeed = 5;
	[SerializeField] private float growSpeed = .5f;
	public float strength;
	private float size = 0;

	[SerializeField] private Rigidbody rb;

	private Vector3 moveDirection;
	private Vector3 initialScale;
	private Vector3 targetScale;

	public Vector3 maxSize;
	public Vector3 minSize;

	public float strengthMax;
	public float strengthMin;

	public GameObject eye;
	public GameObject eyeTarget;
	

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		initialScale = this.transform.localScale;

		t = ((strength - 1) / (strengthMax));
		targetScale = Vector3.Lerp(minSize, maxSize, t);
		//targetScale = initialScale;
	}

	float lerpScale;
	private void Update()
	{
		ProcessInputs();

		if (this.transform.localScale != targetScale)
		{
			lerpScale += growSpeed * Time.deltaTime;
			transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpScale);
		}
		else
		{
			lerpScale = 0;
		}
	}
	void ProcessInputs()
	{
		float moveX = Input.GetAxisRaw("Horizontal");
		float moveZ = Input.GetAxisRaw("Vertical");
		moveDirection = new Vector2(moveX, moveZ);

		//eye.transform.localRotation = Quaternion.RotateTowards(
		//eye.transform.localRotation,
		//Quaternion.LookRotation(new Vector3(-moveDirection.y, 0, moveDirection.x), Vector3.left),
		//Time.deltaTime * 250) ;
		//eyeTarget.transform.localPosition = new Vector3(moveDirection.x, 1, moveDirection.y) * 1.3f;

		eye.transform.localEulerAngles = new Vector4(0, -moveDirection.y, moveDirection.x, moveDirection.y) * 30;

	}

	void FixedUpdate()
	{
		Move();
	}

	private void Move()
	{
		rb.velocity = new Vector2(moveDirection.x, moveDirection.y).normalized * moveSpeed;
	}

	public float t;
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Consumable") && !other.GetComponent<Enemy>().dead && other.GetComponent<Enemy>().strength <= strength)
		{
			Enemy killedEnemy = other.GetComponent<Enemy>();


			strength += killedEnemy.strength /10;
			t = ((strength - 1) / (strengthMax));
		
			targetScale = Vector3.Lerp(minSize, maxSize, t);
			//targetScale = new Vector3(initialScale.x + size / 10, initialScale.y, initialScale.z + size / 10);
			other.GetComponentInChildren<Animator>().SetTrigger("Death");
			killedEnemy.dead = true;
			Destroy(other.gameObject, 2);
		}
		else if(!other.GetComponent<Enemy>().dead)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}

	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawRay(transform.position, new Vector2(moveDirection.x,moveDirection.y) * 10);
	}
}
