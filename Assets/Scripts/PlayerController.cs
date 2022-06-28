using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

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

	public CinemachineVirtualCamera myCam;
	private int lastThreshold;
	[SerializeField] private float targetFov;
	[SerializeField] private float fovUpValue;
	[SerializeField] private float cameraDezoomTime;
	private float timeElapsed = 0;

	private void Start()
	{
		rb = GetComponent<Rigidbody>();
		initialScale = this.transform.localScale;

		t = ((strength - 1) / (strengthMax));
		targetScale = Vector3.Lerp(minSize, maxSize, t);
		targetFov = myCam.m_Lens.OrthographicSize;
		//targetScale = initialScale;
		lastThreshold = (int)strength;
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

		if(myCam.m_Lens.OrthographicSize != targetFov)
        {
			if (timeElapsed < cameraDezoomTime)
			{
				myCam.m_Lens.OrthographicSize = Mathf.Lerp(myCam.m_Lens.OrthographicSize, targetFov, timeElapsed / cameraDezoomTime);
				timeElapsed += Time.deltaTime;
			}
		}

	}
	void ProcessInputs()
	{
		float moveX = Input.GetAxisRaw("Horizontal");
		float moveZ = Input.GetAxisRaw("Vertical");
		moveDirection = new Vector2(moveX, moveZ);
	}

	void FixedUpdate()
	{
		Move();
	}

	private void Move()
	{
		rb.velocity = new Vector3(moveDirection.x, 0, moveDirection.y).normalized * moveSpeed;
	}

	public float t;
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Consumable") && !other.GetComponent<Enemy>().dead && other.GetComponent<Enemy>().strength <= strength)
		{
			Enemy killedEnemy = other.GetComponent<Enemy>();


			strength += killedEnemy.strength /10;
			t = ((strength - 1) / (strengthMax));
			if((int)strength > lastThreshold){
				timeElapsed = 0;
				Debug.Log("Dezoom camera");
				targetFov += fovUpValue;
				lastThreshold = (int)strength;
            }
			targetScale = Vector3.Lerp(minSize, maxSize, t);
			//targetScale = new Vector3(initialScale.x + size / 10, initialScale.y, initialScale.z + size / 10);
			other.GetComponentInChildren<Animator>().SetTrigger("Death");
			killedEnemy.dead = true;
			killedEnemy.target = transform;
			Destroy(other.gameObject, 2);
		}
		else if(!other.GetComponent<Enemy>().dead)
		{
			SceneManager.LoadScene(SceneManager.GetActiveScene().name);
		}
	}
}
