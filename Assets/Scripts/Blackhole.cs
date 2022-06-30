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
    private float attractionRange;
    // Start is called before the first frame update
    void Start()
    {
        targetScale = this.transform.localScale;
        playerTransform = FindObjectOfType<PlayerController>().transform;
        playerScript = FindObjectOfType<PlayerController>();
        chasedTransform = playerTransform;
        attractionRange = this.transform.localScale.x / 2 * 17;
    }

    // Update is called once per frame
    void Update()
    {
        speed = playerScript.moveSpeed * 0.5f;

        if (Vector3.Distance(playerTransform.position, this.transform.position) <= attractionRange)
        {
            playerTransform.position = Vector3.MoveTowards(playerTransform.position, this.transform.position, 0.01f * Time.deltaTime);
        }
        if (this.transform.localScale.magnitude < targetScale.magnitude)
        {
            this.transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpTime * Time.deltaTime);
            attractionRange = this.transform.localScale.x / 2 * 17;
        }
        if (chasedTransform != null)
        {
            transform.position = Vector3.MoveTowards(transform.position, chasedTransform.position, speed*Time.deltaTime);
        }
        else
        {
            chasedTransform = playerTransform;
        }
        if (Vector3.Distance(chasedTransform.position, this.transform.position) < destroyDistance)
        {
            if (chasedTransform.gameObject.CompareTag("Consumable") == false)
            {
                targetScale += chasedTransform.localScale / growDivider;
            }
            if (chasedTransform != playerTransform)
            {
                Destroy(chasedTransform.gameObject);

            }
            else
            {
                playerTransform.gameObject.GetComponent<PlayerController>().Death();
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.GetComponent<GravitationnalPull>() != null && transform.localScale.magnitude > other.transform.localScale.magnitude)
        {
            chasedTransform = other.transform;
            //other.GetComponent<GravitationnalPull>().pullingObject = this.transform;
            other.GetComponent<GravitationnalPull>().darkerHole = this.transform;
        }


    }

    void OnTriggerExit(Collider other)
    {

        if (other.GetComponent<GravitationnalPull>() != null && transform.localScale.magnitude > other.transform.localScale.magnitude)
        {
            chasedTransform = playerTransform;
            //other.GetComponent<GravitationnalPull>().pullingObject = playerTransform;
        }


    }
}
