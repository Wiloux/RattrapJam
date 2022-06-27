using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float growSpeed= .5f;
    private float size = 0;

    [SerializeField] private Rigidbody rb;

    private Vector3 moveDirection;
    private Vector3 initialScale;
    private Vector3 targetScale;
    

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialScale = this.transform.localScale;
        targetScale = initialScale;
    }
    private void Update()
    {
        ProcessInputs();
       if(this.transform.localScale != targetScale)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, targetScale, .5f);
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
        rb.velocity = new Vector3(moveDirection.x * moveSpeed, 0, moveDirection.y * moveSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Consumable"))
        {
            Destroy(other.gameObject);
            Debug.Log("Consumed");
            size++;
            targetScale = new Vector3(initialScale.x + size / 10, initialScale.y, initialScale.z + size / 10);
        }
    }
}
