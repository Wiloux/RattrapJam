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

    public GameObject eye;
    private bool isDead;
    public GameObject deathScreen;

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

        if (myCam.m_Lens.OrthographicSize != targetFov)
        {
            if (timeElapsed < cameraDezoomTime)
            {
                myCam.m_Lens.OrthographicSize = Mathf.Lerp(myCam.m_Lens.OrthographicSize, targetFov, timeElapsed / cameraDezoomTime);
                timeElapsed += Time.deltaTime;
            }
        }

        if (isDead)
        {
            transform.localScale /= 1.05f;
        }

    }
    void ProcessInputs()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector2(moveX, moveZ);

        eye.transform.localEulerAngles = new Vector4(0, -moveDirection.y, moveDirection.x, moveDirection.y) * 30;

    }

    void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        if (!isDead)
            rb.velocity = new Vector3(moveDirection.x, moveDirection.y, 0).normalized * moveSpeed;
    }

    public float t;

    public void UpdateScaleAndStrength(Enemy killedEnemy = null, Debris debris = null)
    {
        if (debris != null)
        {
            strength += debris.strength * 0.1f;
            t = ((strength - 1) / (strengthMax));
        }
        else if (killedEnemy != null)
        {
            strength += killedEnemy.strengthMax * 0.1f;
            t = ((strength - 1) / (strengthMax));
            killedEnemy.dead = true;
        }


        if ((int)strength > lastThreshold)
        {
            timeElapsed = 0;
            Debug.Log("Dezoom camera");
            targetFov += fovUpValue;
            lastThreshold = (int)strength;
        }
        targetScale = Vector3.Lerp(minSize, maxSize, t);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (transform.localScale.magnitude < other.transform.localScale.magnitude && other.CompareTag("Consumable") == false)
        {
            Debug.Log(other.gameObject.name);
            isDead = true;
            deathScreen.SetActive(true);
        }
    }
}
