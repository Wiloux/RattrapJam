using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Start is called before the first frame update
    public Transform blackHoleTransform;
    Rigidbody planetBody;
    [SerializeField] private float influenceRange;
    [SerializeField] private float pullIntensity;
    private Vector3 pullForce;
    private float distanceToPlayer;
    public float strength;
    public float rotationSpd;
    public bool dead;
    public Transform target;
    public List<GameObject> skins = new List<GameObject>();

    public AnimationCurve deathCurve;



    void Start()
    { if (strength <= 1)
		{
            skins[0].SetActive(true);
		}
		else if (strength > 1 && strength <= 2 )
		{
            skins[1].SetActive(true);
        }
        else if (strength > 2 && strength <= 3)
        {
            skins[2].SetActive(true);
        }
        else if(strength > 3)
        {
            skins[3].SetActive(true);
        };
        if (strength <= 1)
		{
            skins[0].SetActive(true);
		}
		else if (strength > 1 && strength <= 2 )
		{
            skins[1].SetActive(true);
        }
        else if (strength > 2 && strength <= 3)
        {
            skins[2].SetActive(true);
        }
        else if(strength > 3)
        {
            skins[3].SetActive(true);
        };

    planetBody = GetComponent<Rigidbody>();
        blackHoleTransform = FindObjectOfType<PlayerController>().transform;
        transform.localScale = Vector3.one * strength;


    }

        if (strength < 3)
        {
            skins[0].SetActive(true);
        }
        else
        {
            skins[1].SetActive(true);
        }
        planetBody = GetComponent<Rigidbody>();
        blackHoleTransform = FindObjectOfType<PlayerController>().transform;
        transform.localScale = Vector3.one * strength;
    }




    float step;
    // Update is called once per frame
    void Update()
    {
    if (dead)
        {
            step +=  0.05f * Time.deltaTime;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, deathCurve.Evaluate(step));
            transform.position = Vector3.Lerp(transform.position, target.position, deathCurve.Evaluate(step));
		}
		  distanceToPlayer = Vector2.Distance(this.transform.position, transform.position);
        if (distanceToPlayer <= influenceRange)
        {
            pullForce = (blackHoleTransform.position - this.transform.position).normalized / distanceToPlayer * pullIntensity;
            planetBody.AddForce(pullForce, ForceMode.Force);
        }


		if (dead)
        {
            step +=  0.05f * Time.deltaTime;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, deathCurve.Evaluate(step));
            transform.position = Vector3.Lerp(transform.position, target.position, deathCurve.Evaluate(step));
		}
        distanceToPlayer = Vector2.Distance(this.transform.position, transform.position);
        if (distanceToPlayer <= influenceRange)
        {
            pullForce = (blackHoleTransform.position - this.transform.position).normalized / distanceToPlayer * pullIntensity;
            planetBody.AddForce(pullForce, ForceMode.Force);
        }
    }

}
