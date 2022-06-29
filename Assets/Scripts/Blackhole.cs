using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackhole : MonoBehaviour
{
    public Transform playerTransform;
    public Transform chasedTransform;
    public float speed;
    public float force = 2;
    public float destroyDistance = 5;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = FindObjectOfType<PlayerController>().transform;
        chasedTransform = playerTransform;
    }

    // Update is called once per frame
    void Update()
    {
        if(chasedTransform != null)
        {
        transform.position = Vector3.MoveTowards(transform.position, chasedTransform.position, speed);
        }
        else
        {
            chasedTransform = playerTransform;
        }
        if(Vector3.Distance(chasedTransform.position, this.transform.position) < destroyDistance)
        {
            if(chasedTransform != playerTransform)
            {
                Destroy(chasedTransform.gameObject);
                
            }
            else
            {
            //Debug.Log("KilledPlayer");
            }
            this.transform.localScale += new Vector3(.2f, .2f, .2f);
        }
    }

    void OnTriggerEnter(Collider other)
    {
      
            if (other.GetComponent<GravitationnalPull>() != null && transform.localScale.magnitude > other.transform.localScale.magnitude)
            {
                chasedTransform = other.transform;
                other.GetComponent<GravitationnalPull>().pullingObject = this.transform;
            }
        
    }

    void OnTriggerExit(Collider other)
    {
       
            if (other.GetComponent<GravitationnalPull>() != null&& transform.localScale.magnitude > other.transform.localScale.magnitude)
            {
            chasedTransform = playerTransform;
                other.GetComponent<GravitationnalPull>().pullingObject = playerTransform;
            }
        
    }
}
