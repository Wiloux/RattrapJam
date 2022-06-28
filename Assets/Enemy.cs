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
    public List<GameObject> skins = new List<GameObject>();
    void Start()
    {
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



    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector2.Distance(this.transform.position, transform.position);
        if (distanceToPlayer <= influenceRange)
        {
            pullForce = (blackHoleTransform.position - this.transform.position).normalized / distanceToPlayer * pullIntensity;
            planetBody.AddForce(pullForce, ForceMode.Force);
        }
    }
}
