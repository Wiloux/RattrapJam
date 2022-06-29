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
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(pulledTarget.position, pullingObject.position);
        if(distanceToPlayer < influenceRange)
        {
            pulledTarget.RotateAround(pullingObject.position, Vector3.up, 20 * Time.deltaTime);
            pullForce = (pullingObject.position - pulledTarget.position).normalized / distanceToPlayer * intensity;
            targetBody.AddForce(pullForce);


            if (Vector3.Distance(transform.localScale, Vector3.zero) > 0.05f)
                transform.localScale -= Vector3.one * (player.strength * playerStrengthPercentage) * Time.deltaTime;
            else
            {
                player.UpdateScaleAndStrength(myEnemy);
                Destroy(gameObject);
            }
        }
        else if (distanceToPlayer > influenceRange + influenceRange/2)
        {
            targetBody.velocity = Vector3.zero;
        }
    }
}
