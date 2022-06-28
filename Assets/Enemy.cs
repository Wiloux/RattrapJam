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
    public List<GameObject> skins = new List<GameObject>();

    public AnimationCurve deathCurve;

    public GravitationnalPull gPull;


    void Start()
    { 
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

        gPull = GetComponent<GravitationnalPull>();
    }





    float step;
    // Update is called once per frame
    void Update()
    {
    if (dead)
        {
            step +=  0.05f * Time.deltaTime;

            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, deathCurve.Evaluate(step));
          //  transform.position = Vector3.Lerp(transform.position, gPull.pullingObject.position, deathCurve.Evaluate(step));
		}

    }

}
